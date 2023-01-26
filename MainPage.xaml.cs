namespace Imagebug;

public partial class MainPage : ContentPage
{
    private Animation m_UpdateTimer ;

    private SkiaSharp.SKColor[] Colours = new SkiaSharp.SKColor[]
        {
            new SkiaSharp.SKColor ( 255, 0, 0 ),
            new SkiaSharp.SKColor ( 255, 128, 0 ),
            new SkiaSharp.SKColor ( 255, 255, 0 ),
            new SkiaSharp.SKColor ( 0, 255, 0 ),
            new SkiaSharp.SKColor ( 0, 0, 255 ),
            new SkiaSharp.SKColor ( 128, 0, 255 )
        } ;

    private const int MAXINDEX = 5 ;
    private const int MARGIN = 64 ;
    private const int RECTWIDTH = 256 ;
    private const int RECTHEIGHT = 64 ;

    private int m_NextIndex ;

    private Image[] m_Images ;

	public MainPage()
	{
		InitializeComponent() ;
        m_UpdateTimer = new Animation ( a=>{}, 1, 2 ) ;

        m_NextIndex = 0 ;
        m_Images = new Image[MAXINDEX+1] ;
        for ( int n = 0 ; n <= MAXINDEX ; n++ )
        m_Images[n] = null ;

        // We start the update timer with only one call on completion.
        uint Millisecs = 500 ;
        m_UpdateTimer.Commit ( this, "UpdateTimer", Millisecs, Millisecs, Easing.Linear, UpdateTimerCompleted, ()=>false ) ;
	}

    /// <summary>Called when the update timer has run to completion.</summary>
    /// <param name="FinalVal">Final value set during the animation.</param>
    /// <param name="Cancelled">If the animation was cancelled.</param>
    private void UpdateTimerCompleted ( double FinalVal, bool Cancelled )
    {
        byte[] JPEG ;

        SkiaSharp.SKBitmap BM = new SkiaSharp.SKBitmap ( 255, RECTHEIGHT ) ;
        using ( SkiaSharp.SKCanvas Cnv = new SkiaSharp.SKCanvas(BM) )
        {
            byte Red = 0 ;
            byte Green = 255 ;

            using ( SkiaSharp.SKPaint Pnt = new SkiaSharp.SKPaint{Color = Colours[m_NextIndex],Style = SkiaSharp.SKPaintStyle.Fill,
                StrokeWidth = 1, IsAntialias=true, TextSize=20 } )
            {
                Cnv.DrawRect ( 0, 0, RECTWIDTH, RECTHEIGHT, Pnt ) ;
            }

            using ( SkiaSharp.SKPaint Pnt = new SkiaSharp.SKPaint{Color = new SkiaSharp.SKColor ( 0, 0, 0 ),Style = SkiaSharp.SKPaintStyle.Fill,
                StrokeWidth = 1, IsAntialias=true, TextSize=20 } )
            {
                Cnv.DrawText ( DateTime.Now.ToString(), 10, 30, Pnt ) ;
            }
        }
        using ( SkiaSharp.SKDynamicMemoryWStream Dst = new SkiaSharp.SKDynamicMemoryWStream () )
        {
            BM.Encode ( Dst, SkiaSharp.SKEncodedImageFormat.Jpeg, 100 ) ;
            SkiaSharp.SKData Dat = Dst.CopyToData () ;
            JPEG = Dat.ToArray () ;
        }

        Image Img = new Image () { Source = ImageSource.FromStream ( () => new System.IO.MemoryStream(JPEG) ) } ;
        AbsoluteLayout.SetLayoutBounds (Img, new Rect (MARGIN, m_NextIndex*RECTHEIGHT, RECTWIDTH, RECTHEIGHT)) ;
        AbsoluteLayout.SetLayoutFlags ( Img, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.None ) ;

        AbsLayout.Children.Add ( Img ) ;

        if ( m_Images[m_NextIndex] != null )
            AbsLayout.Children.Remove ( m_Images[m_NextIndex] ) ;
        m_Images[m_NextIndex] = Img ;

        m_NextIndex ++ ;
        if ( m_NextIndex > MAXINDEX )
            m_NextIndex = 0 ;
        m_UpdateTimer.Commit ( this, "UpdateTimer", 100, 100, Easing.Linear, UpdateTimerCompleted, ()=>false ) ;
    }
}

