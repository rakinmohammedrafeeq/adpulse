namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// ���ߴ糣��
/// </summary>
public static class AdSizeConstants
{
    /// <summary>
    /// �������׼�ߴ�
    /// </summary>
    public static class Banner
    {
        /// <summary>
        /// 320x50 - �ƶ����
        /// </summary>
        public static readonly (int Width, int Height) MobileBanner = (320, 50);

        /// <summary>
        /// 728x90 - ���а���λ
        /// </summary>
        public static readonly (int Width, int Height) Leaderboard = (728, 90);

        /// <summary>
        /// 300x250 - �еȾ���
        /// </summary>
        public static readonly (int Width, int Height) MediumRectangle = (300, 250);

        /// <summary>
        /// 336x280 - �����
        /// </summary>
        public static readonly (int Width, int Height) LargeRectangle = (336, 280);

        /// <summary>
        /// 160x600 - ��Ħ���¥
        /// </summary>
        public static readonly (int Width, int Height) WideSkyscraper = (160, 600);

        /// <summary>
        /// 120x600 - Ħ���¥
        /// </summary>
        public static readonly (int Width, int Height) Skyscraper = (120, 600);
    }

    /// <summary>
    /// ��Ƶ����׼�ߴ�
    /// </summary>
    public static class Video
    {
        /// <summary>
        /// 640x480 - ����
        /// </summary>
        public static readonly (int Width, int Height) SD = (640, 480);

        /// <summary>
        /// 1280x720 - ����
        /// </summary>
        public static readonly (int Width, int Height) HD = (1280, 720);

        /// <summary>
        /// 1920x1080 - ȫ����
        /// </summary>
        public static readonly (int Width, int Height) FullHD = (1920, 1080);

        /// <summary>
        /// 3840x2160 - 4K������
        /// </summary>
        public static readonly (int Width, int Height) UHD = (3840, 2160);
    }

    /// <summary>
    /// ԭ������׼�ߴ�
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// 1200x627 - �罻ý�����
        /// </summary>
        public static readonly (int Width, int Height) SocialCover = (1200, 627);

        /// <summary>
        /// 1080x1080 - ������
        /// </summary>
        public static readonly (int Width, int Height) Square = (1080, 1080);

        /// <summary>
        /// 1080x1350 - ����
        /// </summary>
        public static readonly (int Width, int Height) Portrait = (1080, 1350);
    }
}