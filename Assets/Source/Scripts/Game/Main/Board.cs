using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Attributes;
using Core.Settings;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class Board : DIBehaviour
{
    [SerializeField]
    private LevelController _levelController;

    [SerializeField]
    private GameObject _boardModel;

    [SerializeField]
    private Transform _blocksParent;

    [Inject]
    private GameSettings _gameSettings;

    private Block[] _blocks;
    private int2 _dimensions;
    private Queue<Block> _mergedBlocks;
    private Queue<Block> _blocksToDestroy;
    private Queue<Block> _blocksToSpawn;
    private Pair<bool, Direction> _queuedSwipeDirection;
    private bool _animating;

    protected override void OnAppInitialized()
    {
        _mergedBlocks = new Queue<Block>();
        _blocksToDestroy = new Queue<Block>();
        _blocksToSpawn = new Queue<Block>();
        _queuedSwipeDirection = new Pair<bool, Direction>(false, Direction.Down);
    }

    public void SetupBoard(LevelConfig levelConfig, LevelData savedData = null)
    {
        ClearBoard();
        CancelInvoke(nameof(OnSetupAnimationEnd));
        
        _dimensions = levelConfig.Dimensions;
        _blocks = new Block[_dimensions.x * _dimensions.y];

        Block block = null;
        float animationDelay = .2f;
        
        foreach (var cell in savedData == null ? levelConfig.CellsData : savedData.BlocksData)
        {
            block = _levelController.BlocksPool.Spawn();
            block.SetBlock(cell.BlockInfo);
            block.transform.parent = _blocksParent;
            block.Coords = cell.Coords;
            
            Vector3 position = GetCellPosition(cell.Coords);
            block.transform.localPosition = position - Vector3.forward * 15f;

            block.transform.DOLocalMoveZ(0f, 1f).SetDelay(animationDelay);

            animationDelay += .2f;
            _blocks[cell.Coords.Index(_dimensions.x)] = block;
        }
        
        Invoke(nameof(OnSetupAnimationEnd), animationDelay + 1f);

        _boardModel.transform.localScale = new Vector3(_dimensions.x * _gameSettings.CellSize.x,
            _dimensions.y * _gameSettings.CellSize.y, _boardModel.transform.localScale.z);
    }

    private void OnSetupAnimationEnd()
    {
        Dispatch(NotificationType.BoardSetupComplete);
    }

    public bool Swipe(Direction direction)
    {
        if (_animating)
        {
            _queuedSwipeDirection.First = true;
            _queuedSwipeDirection.Second = direction;
            return false;
        }
        
        Coords coords = new Coords();

        bool anyChanges = false;
        
        switch (direction)
        {
            case Direction.Down:
            {
                for (coords.y = 1; coords.y < _dimensions.y; coords.y++)
                {
                    for (coords.x = 0; coords.x < _dimensions.x; coords.x++)
                    {
                        HandleBlock();
                    }
                }

                break;
            }
            case Direction.Up:
            {
                for (coords.y = _dimensions.y - 2; coords.y >= 0; coords.y--)
                {
                    for (coords.x = 0; coords.x < _dimensions.x; coords.x++)
                    {
                        HandleBlock();
                    }
                }

                break;
            }
            case Direction.Right:
            {
                for (coords.x = _dimensions.x - 2; coords.x >= 0; coords.x--)
                {
                    for (coords.y = 0; coords.y < _dimensions.y; coords.y++)
                    {
                        HandleBlock();
                    }
                }

                break;
            }
            case Direction.Left:
            {
                for (coords.x = 1; coords.x < _dimensions.y; coords.x++)
                {
                    for (coords.y = 0; coords.y < _dimensions.y; coords.y++)
                    {
                        HandleBlock();
                    }
                }

                break;
            }
        }

        void HandleBlock()
        {
            Block block = _blocks[coords.Index(_dimensions.x)];
            if (block == null)
            {
                return;
            }

            if (block.Movable == false)
            {
                if (block.Mergeable)
                {
                    block.Shake();
                }
                
                return;
            }

            Block hit = SwipeBlock(block, direction, out anyChanges);
            anyChanges = TryMergeBlocks(block, hit) || anyChanges;

            if (block.Coords.Equals(coords) == false)
            {
                block.transform.DOLocalMove(GetCellPosition(block.Coords), _gameSettings.SwipeAnimationDuration);
            }
        }

        ClearMergedBlocks();

        _animating = true;
        Invoke(nameof(OnSwipeAnimationEnd), _gameSettings.SwipeAnimationDuration);
        return anyChanges;
    }

    private Block SwipeBlock(Block block, Direction direction, out bool moved)
    {
        Coords target = new Coords();
        Block hit = null;
        
        if (direction == Direction.Down)
        {
            target.x = block.Coords.x;
            for (target.y = block.Coords.y; ; target.y--)
            {
                if (target.y == 0)
                {
                    break;
                }
                
                hit = _blocks[Coords.Index(target.x, target.y - 1, _dimensions.x)];
                if (hit != null)
                {
                    break;
                }
            }
        }
        if (direction == Direction.Up)
        {
            target.x = block.Coords.x;
            for (target.y = block.Coords.y; ; target.y++)
            {
                if (target.y == _dimensions.y - 1)
                {
                    break;
                }
                
                hit = _blocks[Coords.Index(target.x, target.y + 1, _dimensions.x)];
                if (hit != null)
                {
                    break;
                }
            }
        }
        if (direction == Direction.Right)
        {
            target.y = block.Coords.y;
            for (target.x = block.Coords.x; ; target.x++)
            {
                if (target.x == _dimensions.x - 1)
                {
                    break;
                }
                
                hit = _blocks[Coords.Index(target.x + 1, target.y, _dimensions.x)];
                if (hit != null)
                {
                    break;
                }
            }
        }
        if (direction == Direction.Left)
        {
            target.y = block.Coords.y;
            for (target.x = block.Coords.x; ; target.x--)
            {
                if (target.x == 0)
                {
                    break;
                }
                
                hit = _blocks[Coords.Index(target.x - 1, target.y, _dimensions.x)];
                if (hit != null)
                {
                    break;
                }
            }
        }

        moved = block.Coords.Equals(target) == false;
        MoveBlockTo(block, target);
        return hit;
    }

    private void MoveBlockTo(Block block, Coords target)
    {
        _blocks[block.Coords.Index(_dimensions.x)] = null;
        _blocks[target.Index(_dimensions.x)] = block;
        block.Coords = target;
    }
    
    private bool TryMergeBlocks(Block block, Block targetBlock)
    {
        if (block == null || targetBlock == null ||
            block.Mergeable == false || targetBlock.Mergeable == false || 
            block.JustMerged || targetBlock.JustMerged ||
            block.PowerOfTwo != targetBlock.PowerOfTwo)
        {
            return false;
        }

        bool mergeWithFrozen = block.Movable == false || targetBlock.Movable == false;

        _blocks[block.Coords.Index(_dimensions.x)] = null;
        _blocks[targetBlock.Coords.Index(_dimensions.x)] = null;

        int mergedPOT = targetBlock.PowerOfTwo + 1;
        
        Block mergedBlock = _levelController.BlocksPool.Spawn();
        mergedBlock.SetBlock(mergedPOT, true, true);
        mergedBlock.transform.parent = _blocksParent;
        mergedBlock.Coords = targetBlock.Coords;
        mergedBlock.transform.localPosition = GetCellPosition(mergedBlock.Coords);
        
        mergedBlock.JustMerged = true;
        _mergedBlocks.Enqueue(mergedBlock);
        
        _blocks[mergedBlock.Coords.Index(_dimensions.x)] = mergedBlock;
        mergedBlock.gameObject.SetActive(false);
        _blocksToSpawn.Enqueue(mergedBlock);
        
        block.Coords = mergedBlock.Coords;
        _blocksToDestroy.Enqueue(block);

        targetBlock.Coords = mergedBlock.Coords;
        _blocksToDestroy.Enqueue(targetBlock);


        if (mergeWithFrozen)
        {
            mergedBlock.PlayFrozenMergeVFX();
        }
        
        Dispatch(NotificationType.BlocksMerge, NotificationParams.Get(mergedPOT));
        
        return true;
    }

    private void OnSwipeAnimationEnd()
    {
        _animating = false;
        while (_blocksToSpawn.Count > 0)
        {
            _blocksToSpawn.Dequeue().gameObject.SetActive(true);
        }

        while (_blocksToDestroy.Count > 0)
        {
            _levelController.BlocksPool.Despawn(_blocksToDestroy.Dequeue());
        }

        if (_queuedSwipeDirection.First)
        {
            _queuedSwipeDirection.First = false;
            Swipe(_queuedSwipeDirection.Second);
        }
    }
    
    private void ClearMergedBlocks()
    {
        while (_mergedBlocks.Count > 0)
        {
            _mergedBlocks.Dequeue().JustMerged = false;
        }
    }

    private Vector2 GetCellPosition(Coords coords)
    {
        float x = coords.x * _gameSettings.CellSize.x - (_dimensions.x - 1) * _gameSettings.CellSize.x * .5f;
        float y = coords.y * _gameSettings.CellSize.y - (_dimensions.y - 1) * _gameSettings.CellSize.y * .5f;
        return new Vector2(x, y);
    }

    private void ClearBoard()
    {
        if (_blocks == null)
        {
            return;
        }

        for (int i = 0; i < _blocks.Length; i++)
        {
            if (_blocks[i] == null)
            {
                continue;
            }
            
            _levelController.BlocksPool.Despawn(_blocks[i]);
        }

        _blocks = null;
    }

    public List<BoardCellData> GetBlocksData()
    {
        return _blocks.Where(block => block != null).Select(block => block.GetBlockData()).ToList();
    }
}