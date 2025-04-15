using System;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnProgressChangeEventArgs> OnProgressChanged;
    public class OnProgressChangeEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }
}
