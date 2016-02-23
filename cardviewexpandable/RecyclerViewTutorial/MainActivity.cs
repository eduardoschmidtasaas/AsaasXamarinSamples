using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views.Animations;
using Android.Animation;
using System.Timers;
using System.Threading.Tasks;

namespace RecyclerViewTutorial
{
    [Activity(Label = "CardView Expandable", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private RecyclerView mRecyclerView;
        private RecyclerView.LayoutManager mLayoutManager;
        private RecyclerView.Adapter mAdapter;
		private List<Data> mDatas;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mDatas = new List<Data>();
			mDatas.Add(new Data() { Name = "CardView 1", Subject = "Car", Message = "Your car message", imageId = Resource.Drawable.ic_car, eventHandler = callBackTest });
			mDatas.Add(new Data() { Name = "CardView 2", Subject = "Motorcycle", Message = "Your motorcycle message", imageId = Resource.Drawable.ic_motorcycle, eventHandler = callBackTest });
			mDatas.Add(new Data() { Name = "CardView 3", Subject = "Plane", Message = "Your plane message", imageId = Resource.Drawable.ic_airplane, eventHandler = callBackTest });

            //Create our layout manager
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
			mAdapter = new CustomRecyclerAdapter(mDatas, mRecyclerView, this);
            mRecyclerView.SetAdapter(mAdapter);
        }

		int count = 3;
		public void callBackTest(object sender, EventArgs e){
			count += 1;

			Button btn = (Button)sender;
			btn.Text = "Click " + count;
			Console.WriteLine ("Button Click Test " + btn.Text);
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionbar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

		int cardCount = 3;
		public void generateDataTest(){
			cardCount += 1;
			Random rnd = new Random();

			int nRandom = rnd.Next(1, 3);
			switch(nRandom){
			case 1:
				mDatas.Add (new Data () {
					Name = "CardView "+ cardCount,
					Subject = "Car",
					Message = "Your car message",
					imageId = Resource.Drawable.ic_car,
					eventHandler = callBackTest
				});
				break;
			case 2:
				mDatas.Add (new Data () {
					Name = "CardView "+ cardCount,
					Subject = "Motorcycle",
					Message = "Your motorcycle message",
					imageId = Resource.Drawable.ic_motorcycle,
					eventHandler = callBackTest
				});
				break;
			case 3:
				mDatas.Add (new Data () {
					Name = "CardView "+ cardCount,
					Subject = "Plane",
					Message = "Your plane message",
					imageId = Resource.Drawable.ic_airplane,
					eventHandler = callBackTest
				});
				break;
			}
			mAdapter.NotifyItemInserted(mDatas.Count - 1);
		}
			
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
			case Resource.Id.add:
				generateDataTest ();
				break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}

