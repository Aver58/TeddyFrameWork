using System;

namespace TsiU
{
    public class TStaticHelperBase<T> : Singleton<T> where T : class, new()
	{
		//----------------------------------------------------------
		public void Init() 
        {
			onInit();
			hasInited = true;
		}
		public void Uninit() 
        {
			onUninit();
			hasInited = false;
		}
		protected virtual void onInit() 	{}
		protected virtual void onUninit()	{}
		//----------------------------------------------------------
        protected bool hasInited
        {
            private set;
            get;
        }
	}
}

