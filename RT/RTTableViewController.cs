﻿
using System;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace RT
{
	public class RTTableViewController : UITableViewController
	{
		private readonly UINavigationController navControl;
		private readonly RTRepository repository = new RTRepository();

		private RTTableViewSource source; 

		public RTTableViewController (UINavigationController navControl) 
		{
			this.navControl = navControl;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Register the TableView's data source
			source = new RTTableViewSource ();
			TableView = new UITableView(Rectangle.Empty) {Source = source};
			RefreshControl = new UIRefreshControl();
			RefreshControl.ValueChanged += RefreshControlOnValueChanged;
			source.OnRowSelect = OnRowSelect; 
		}

		private void OnRowSelect(int section, int row)
		{
			IMovie movie=null;
			switch (section) {
			case 0:
				movie = source.openingMovies.movies [row];
				break; 
			case 1:
				movie = source.topBox.movies [row];
				break;
			case 2:
				movie = source.inTheaters.movies [row];
				break;
			default:
				Console.WriteLine ("Error on row select");
				break;
			}
			RTMovieView movieView = new RTMovieView ();
			RootElement r = movieView.getUI (movie);

			navControl.PushViewController(new RTMovieView(), true);
		}

		public async override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			await LoadMoviesAsync ();
		}

		private async void RefreshControlOnValueChanged(object sender, EventArgs eventArgs)
		{
			await LoadMoviesAsync();
			RefreshControl.EndRefreshing();
		}

		private async Task LoadMoviesAsync()
		{
			var topBox 		  = await repository.RetrieveTopBox();
			var inTheaters 	  = await repository.RetrieveInTheaters ();
			var openingMovies = await repository.RetrieveOpeningMovies ();
			
			source.openingMovies = openingMovies;
			source.topBox        = topBox;
			source.inTheaters    = inTheaters;

			TableView.ReloadData();
		}
	}
}