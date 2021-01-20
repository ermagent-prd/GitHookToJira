﻿using System;
using Unity;

namespace GeminiTools.Container
{
    internal static class GeminiContainer
    {
        #region Public properties
        public readonly static Lazy<IUnityContainer> DefaultInstance =
             new Lazy<IUnityContainer>(
                 () => GeminiContainerFactory.Execute());
        #endregion

    }
}