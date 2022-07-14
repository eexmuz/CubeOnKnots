using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SequenceCommand : DIBehaviour
    {
        #region Fields

        private readonly List<BaseCommand> _commands = new List<BaseCommand>();

        #endregion

        #region Public Methods and Operators

        public static void Create<T>()
        {
            new GameObject("SequenceCommand", typeof(T));
        }

        public void AddCommand(BaseCommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            NextCommand();
        }

        #endregion

        #region Methods

        protected override void OnAppInitialized()
        {
            base.OnAppInitialized();

            DontDestroyOnLoad(gameObject);
            Execute();
        }

        private void NextCommand()
        {
            if (_commands.Count < 1)
            {
                _commands.Clear();
                Destroy(gameObject);
                return;
            }

            var command = _commands[0];
            Debug.Log("Next command => " + command);
            _commands.RemoveAt(0);
            command.Complete += OnCommandComplete;
            command.Execute();
        }

        private void OnCommandComplete(bool success)
        {
            if (success)
            {
                NextCommand();
            }
            else
            {
                _commands.Clear();
                Destroy(gameObject);
            }
        }

        #endregion
    }
}