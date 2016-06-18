#pragma warning disable 0618
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zios.Events{
	[Serializable]
	public class EventStepper{
		[NonSerialized] public static List<EventStepper> instances = new List<EventStepper>();
		[NonSerialized] public static EventStepper active;
		[NonSerialized] public static string title;
		[NonSerialized] public static string message;
		public Method onComplete = ()=>{};
		public MethodStep method;
		public IList collection;
		public int index;
		public int passes = 1;
		public bool complete;
		public EventStepper(MethodStep method,Method onComplete,IList collection,int passCount=1){
			this.method = method;
			this.collection = collection;
			this.passes = passCount;
			this.onComplete = onComplete ?? this.onComplete;
			EventStepper.instances.AddNew(this);
		}
		public void Step(){
			EventStepper.active = this;
			var count = this.passes;
			while(count > 0){
				count -= 1;
				bool canceled = false;
				if(this.index != -1){
					this.method(this.collection,this.index);
					float percent = ((float)this.index)/this.collection.Count;
					canceled = Utility.DisplayCancelableProgressBar(EventStepper.title,EventStepper.message,percent);
					this.index += 1;
				}
				bool loading = Application.isLoadingLevel;
				bool ended = (this.index > this.collection.Count-1) || this.index == -1;
				if((loading || canceled || ended) && !this.complete){
					this.index = -1;
					this.complete = true;
					this.onComplete();
					EventStepper.instances.Remove(this);
					Utility.ClearProgressBar();
					break;
				}
			}
			EventStepper.active = null;
		}
	}
}