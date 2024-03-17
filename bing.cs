using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

class Program
{
    // Declare the SystemParametersInfo function
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    // Constants used for the SystemParametersInfo function
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    static void Main(string[] args)
    {
        string bingImageUrl = GetBingWallpaperUrl();
        if (bingImageUrl != null)
        {
            string localImagePath = DownloadImage(bingImageUrl);
            if (localImagePath != null)
            {
                SetWallpaper(localImagePath);
                Console.WriteLine("Wallpaper set successfully.");
            }
            else
            {
                Console.WriteLine("Failed to download the image.");
            }
        }
        else
        {
            Console.WriteLine("Failed to get Bing wallpaper URL.");
        }
    }

    static string GetBingWallpaperUrl()
    {
        string bingUrl = "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US";
        try
        {
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(bingUrl);
                // Parse JSON to get the image URL
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                string imageUrl = "https://www.bing.com" + data.images[0].url;
                return imageUrl;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting Bing wallpaper URL: " + ex.Message);
            return null;
        }
    }

    static string DownloadImage(string imageUrl)
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                string fileName = Path.GetFileName(imageUrl);
                string localImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), fileName);
                client.DownloadFile(imageUrl, localImagePath);
                return localImagePath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error downloading image: " + ex.Message);
            return null;
        }
    }

    static void SetWallpaper(string imagePath)
    {
        // Set the desktop wallpaper using SystemParametersInfo function
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }
}

