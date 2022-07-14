using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Utility
{
    public class ContinuationManager
    {
        #region Static Fields

        private static readonly List<Job> Jobs = new List<Job>();

        #endregion

        #region Public Methods and Operators

        public static void Add(Func<bool> completed, Action continueWith)
        {
            if (!Jobs.Any())
                EditorApplication.update += Update;

            Jobs.Add(new Job(completed, continueWith));
        }

        #endregion

        #region Methods

        private static void Update()
        {
            for (var i = 0; i >= 0; --i)
            {
                var jobIt = Jobs[i];
                if (jobIt.Completed())
                {
                    jobIt.ContinueWith();
                    Jobs.RemoveAt(i);
                }
            }
            if (!Jobs.Any())
                EditorApplication.update -= Update;
        }

        #endregion

        private class Job
        {
            #region Constructors and Destructors

            public Job(Func<bool> completed, Action continueWith)
            {
                Completed = completed;
                ContinueWith = continueWith;
            }

            #endregion

            #region Public Properties

            public Func<bool> Completed { get; }
            public Action ContinueWith { get; }

            #endregion
        }
    }
}