using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;

namespace external_files
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ImageView photo;
        Button shoot;
        TextView text;
        int n;

        bool MExternalStorageAvailable;
        bool MExternalStorageWriteable;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            photo = (ImageView)FindViewById(Resource.Id.photo);
            shoot = (Button)FindViewById(Resource.Id.takePhoto);
            text = (TextView)FindViewById(Resource.Id.textView1);
            shoot.Click += Shoot_Click;
            MExternalStorageAvailable = MExternalStorageWriteable = false;
            n = 0;
        }

        private void Shoot_Click(object sender, System.EventArgs e)
        {
            SetPermissitions();
            Intent i = new Intent(Android.Provider.MediaStore.ActionImageCapture);
            StartActivityForResult(i, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    Bitmap bimap = (Bitmap)data.Extras.Get("data");
                    photo.SetImageBitmap(bimap);
                    SaveImageToExternalStorage(bimap);
                }
            }
        }

        private void SaveImageToExternalStorage(Bitmap bimap)
        {
            string root = GetExternalMediaDirs()[0].ToString();
            Java.IO.File myDir = new Java.IO.File(root + "/saved_images");
            myDir.Mkdir();
            string fname = "Image-" + n + ".jpg";
            Java.IO.File file = new Java.IO.File(myDir, fname);
            if (file.Exists())
            {
                file.Delete();
            }
            try
            {
                string path = System.IO.Path.Combine(myDir.AbsolutePath, fname);
                FileStream fs = new FileStream(path, FileMode.Create);
                if (fs != null)
                {
                    bimap.Compress(Bitmap.CompressFormat.Png, 90, fs);
                    text.Text = myDir.AbsolutePath;
                }
                fs.Flush();
                fs.Close();
            }
            catch (Exception e)
            {
                text.Text = e.ToString();
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
            }
        }

        public void SetPermissitions()
        {
            string state = Android.OS.Environment.ExternalStorageState;
            if (Android.OS.Environment.MediaMounted.Equals(state))
            {
                MExternalStorageAvailable = MExternalStorageWriteable = true;
                Toast.MakeText(this, "we can read and write the media", ToastLength.Long).Show();
            }
            else if (Android.OS.Environment.MediaMountedReadOnly.Equals(state))
            {
                MExternalStorageAvailable = true;
                MExternalStorageWriteable = false;
                Toast.MakeText(this, "we can read the media", ToastLength.Long).Show();
            }
            else
            {
                MExternalStorageAvailable = MExternalStorageWriteable = false;
                Toast.MakeText(this, "we can't read and write the media", ToastLength.Long).Show();
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}