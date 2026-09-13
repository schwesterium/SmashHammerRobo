/*
Author : schwesterium
Date   : 2026/08/01
*/

namespace SchwesteriumLibrary.Input
{
    public interface IInputHandlerOwner<T> where T : MultiPlayInputHandlerBase
    {
        public T GetInputHandler();
    }
}