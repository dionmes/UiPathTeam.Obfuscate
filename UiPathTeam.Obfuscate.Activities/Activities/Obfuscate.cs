using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using UiPathTeam.Obfuscate.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using ImageMagick;
using System.IO;

namespace UiPathTeam.Obfuscate.Activities
{
    [LocalizedDisplayName(nameof(Resources.Obfuscate_DisplayName))]
    [LocalizedDescription(nameof(Resources.Obfuscate_Description))]
    public class Obfuscate : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
        [LocalizedDescription(nameof(Resources.Timeout_Description))]
        public InArgument<int> TimeoutMS { get; set; } = 60000;

        [LocalizedDisplayName(nameof(Resources.Obfuscate_InputImage_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_InputImage_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<Image> InputImage { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_PositionX_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_PositionX_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> PositionX { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_PositionY_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_PositionY_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> PositionY { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_Width_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_Width_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Width { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_Height_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_Height_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> Height { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_OutputImage_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_OutputImage_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<Image> OutputImage { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_Blur_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_Blur_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<bool> Blur { get; set; }

        [LocalizedDisplayName(nameof(Resources.Obfuscate_BlurAmount_DisplayName))]
        [LocalizedDescription(nameof(Resources.Obfuscate_BlurAmount_Description))]
        [LocalizedCategory(nameof(Resources.Options_Category))]
        public InArgument<int> BlurAmount { get; set; }

        #endregion


        #region Constructors

        public Obfuscate()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (InputImage == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(InputImage)));
            if (PositionX == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(PositionX)));
            if (PositionY == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(PositionY)));
            if (Width == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Width)));
            if (Height == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Height)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var timeout = TimeoutMS.Get(context);
            var inputImage = InputImage.Get(context);
            var positionX = PositionX.Get(context);
            var positionY = PositionY.Get(context);
            var width = Width.Get(context);
            var height = Height.Get(context);
            var blur = Blur.Get(context);
            var blurAmount = BlurAmount.Get(context);

            // Set a timeout on the execution
            var task = ExecuteWithTimeout(context, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task) throw new TimeoutException(Resources.Timeout_Error);

            Image returnImage;

            // Check if activity should blur or hide part of the image

            if (blur)
            {

                // Convert image to bytestream
                ImageConverter _imageConverter = new ImageConverter();
                byte[] imageByteStream = (byte[])_imageConverter.ConvertTo(inputImage, typeof(byte[]));

                // Create image from bytestream for MagickImage use
                var magickimage = new MagickImage(imageByteStream);

                // Blur part of image
                magickimage.RegionMask(new MagickGeometry(positionX, positionY, width, height));
                magickimage.GaussianBlur(blurAmount, 25);
                magickimage.RemoveRegionMask();

                // Convert MagickInmage to Bytestream
                var imageBytesOut = magickimage.ToByteArray();
                MemoryStream ms = new MemoryStream(imageBytesOut);

                // Create return image from bytestream
                returnImage = Image.FromStream(ms);

            }
            else
            {

                // Create graphics context with returnImage
                returnImage = inputImage;

                using (Graphics g = Graphics.FromImage(returnImage))
                {
                    // Define brush and rectangle
                    SolidBrush blackBrush = new SolidBrush(Color.Black);
                    Rectangle rect = new Rectangle(positionX, positionY, width, height);

                    // Fill rectangle
                    g.FillRectangle(blackBrush, rect);

                    // Cleanup
                    g.Dispose();
                }

            }

            // Outputs
            return (ctx) => {
                OutputImage.Set(ctx, returnImage);
            };
        }

        private async Task ExecuteWithTimeout(AsyncCodeActivityContext context, CancellationToken cancellationToken = default)
        {
            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
        }

        #endregion
    }
}

