using System;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using System.Timers;
using Android.Animation;
using Android.Views.Animations;
using Android.Views;

namespace RecyclerViewTutorial
{
	public class CustomRecyclerAdapter : RecyclerView.Adapter
	{
		private List<Data> mEmails;
		private RecyclerView mRecyclerView;
		private Context mContext;
		private int mCurrentPosition = -1;
		private RelativeLayout layoutInformations;
		private ImageView imageArrow;
		private Timer timer;
		private ValueAnimator mAnimator;
		private Animation rotateAnim;


		public CustomRecyclerAdapter(List<Data> emails, RecyclerView recyclerView, Context context)
		{
			mEmails = emails;
			mRecyclerView = recyclerView;
			mContext = context;
		}

		public class MyView : RecyclerView.ViewHolder
		{
			public View mMainView { get; set; }
			public TextView mName { get; set; }
			public TextView mSubject { get; set; }
			public TextView mMessage { get; set; }
			public ImageView mImageDoc { get; set; }
			public Button mButtonSend { get; set; }


			public MyView (View view) : base(view)
			{
				mMainView = view;
			}
		}

		public override int GetItemViewType(int position)
		{
			return Resource.Layout.row;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.row, parent, false);

			TextView txtName = row.FindViewById<TextView>(Resource.Id.txtName);
			TextView txtSubject = row.FindViewById<TextView>(Resource.Id.txtSubject);
			TextView txtMessage = row.FindViewById<TextView>(Resource.Id.requisitsText);
			ImageView imageDoc = row.FindViewById<ImageView> (Resource.Id.icon_doc);
			Button buttonSend = row.FindViewById<Button> (Resource.Id.btnSend);

			MyView view = new MyView(row) { mName = txtName, mSubject = txtSubject, mMessage = txtMessage, mImageDoc = imageDoc, mButtonSend = buttonSend };
			return view;

		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyView myHolder = holder as MyView;
			myHolder.mMainView.Click -= mMainView_Click;
			myHolder.mMainView.Click += mMainView_Click;
			myHolder.mName.Text = mEmails[position].Name;
			myHolder.mSubject.Text = mEmails[position].Subject;
			myHolder.mMessage.Text = mEmails[position].Message;
			myHolder.mImageDoc.SetImageResource(mEmails[position].imageId);
			myHolder.mButtonSend.Click -= mEmails [position].eventHandler;
			myHolder.mButtonSend.Click += mEmails [position].eventHandler;

			if (position > mCurrentPosition)
			{
				int currentAnim = Resource.Animation.slide_left_to_right;
				SetAnimation(myHolder.mMainView, currentAnim);
				mCurrentPosition = position;
			}     
		}

		private void SetAnimation(View view, int currentAnim)
		{
			Animator animator = AnimatorInflater.LoadAnimator(mContext, Resource.Animation.flip);
			animator.SetTarget(view);
			animator.Start();
			Animation anim = AnimationUtils.LoadAnimation(mContext, currentAnim);
			view.StartAnimation(anim);
		}

		bool visible = false;
		private void mMainView_Click(object sender, EventArgs e)
		{
			View view = (View)sender;
			int position = mRecyclerView.GetChildPosition((View)sender);
			layoutInformations = view.FindViewById<RelativeLayout> (Resource.Id.viewx);
			imageArrow = view.FindViewById<ImageView>(Resource.Id.parent_list_item_expand_arrow);
			if (layoutInformations.Visibility == ViewStates.Gone) {
				expand();
				timer = new Timer (200);
				timer.AutoReset = false;
				timer.Elapsed += delegate {
					mRecyclerView.Handler.Post(() => {
						mRecyclerView.ScrollToPosition (position);
					});
				};
				timer.Start ();

			} else {
				collapse();
			}
		}

		public override int ItemCount
		{
			get { return mEmails.Count; }
		}

		private void expand() {
			layoutInformations.Visibility = ViewStates.Visible;
			int widthSpec = View.MeasureSpec.MakeMeasureSpec(0,	MeasureSpecMode.Unspecified);
			int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
			layoutInformations.Measure(widthSpec, heightSpec);

			mAnimator = slideAnimator(0, layoutInformations.MeasuredHeight);
			mAnimator.Start();
			rotateAnimation (0, 180);
		}

		private ValueAnimator slideAnimator(int start, int end) {
			if (mAnimator != null) {
				mAnimator.Cancel ();;
				mAnimator = null;
			}			
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.SetDuration (200);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) => {
				ValueAnimator valueAnimator = e.Animation;
				int value = (Int32) valueAnimator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = layoutInformations.LayoutParameters;
				layoutParams.Height = value;
				layoutInformations.LayoutParameters = layoutParams;
			};
			return animator;
		}

		private void rotateAnimation(int start, int end) {
			if (rotateAnim != null) {
				rotateAnim.Cancel ();;
				rotateAnim = null;
			}
			rotateAnim = new RotateAnimation(start,
				end,
				Dimension.RelativeToSelf, 0.5f,
				Dimension.RelativeToSelf, 0.5f);

			rotateAnim.Duration = 200;
			rotateAnim.FillAfter = true;
			imageArrow.StartAnimation (rotateAnim);
		}

		private void collapse() {
			rotateAnimation (180, 0);
			int finalHeight = layoutInformations.Height;
			mAnimator = slideAnimator(finalHeight, 0);
			mAnimator.Start();
			if (timer != null) {
				timer.Stop ();
				timer = null;
			}

			timer = new Timer (200);
			timer.AutoReset = false;
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				layoutInformations.Handler.Post (() => {
					layoutInformations.Visibility = ViewStates.Gone;
				});
			};
			timer.Start ();
		}
	}
}

