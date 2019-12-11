using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace Aoto.EMS.Common
{
    public class QrCodeFactory
    {
        #region 二维码生成器
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static Bitmap CreateQRCode(string content)
        {
            try
            {
                QRCodeEncoder qrEncoder = new QRCodeEncoder();
                //二维码类型
                qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                //二维码尺寸
                qrEncoder.QRCodeScale = 4;
                //二维码版本
                qrEncoder.QRCodeVersion = 7;
                //二维码容错程度
                qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                //字体与背景颜色
                qrEncoder.QRCodeBackgroundColor = Color.White;
                qrEncoder.QRCodeForegroundColor = Color.Black;
                //UTF-8编码类型
                Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);

                return qrcode;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Bitmap CreateQRCode(string content,int scale)
        {
            try
            {
                QRCodeEncoder qrEncoder = new QRCodeEncoder();
                //二维码类型
                qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                //二维码尺寸
                qrEncoder.QRCodeScale = scale;
                //二维码版本
                qrEncoder.QRCodeVersion = 7;
                //二维码容错程度
                qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                //字体与背景颜色
                qrEncoder.QRCodeBackgroundColor = Color.White;
                qrEncoder.QRCodeForegroundColor = Color.Black;
                //UTF-8编码类型
                Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);

                return qrcode;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 生成带logo二维码
        /// </summary>
        /// <returns></returns>
        public static Bitmap CreateQRCodeWithLogo(string content, string logopath)
        {
            //生成二维码
            Bitmap qrcode = CreateQRCode(content);
            //生成logo
            Bitmap logo = new Bitmap(logopath);
            //合成
            ImageUtility util = new ImageUtility();
            Bitmap finalImage = util.MergeQrImg(qrcode, logo);
            return finalImage;
        }
        /// <summary>
        /// 保存二维码
        /// </summary>
        /// <param name="QRCode">二维码图片</param>
        /// <param name="SavePath">保存路径</param>
        /// <param name="QRCodeName">图片名称</param>
        public static void SaveQRCode(Bitmap QRCode, string SavePath, string QRCodeName)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            QRCode.Save(Path.Combine(SavePath, QRCodeName + ".png"), ImageFormat.Png);

            QRCode.Dispose();
        }

        #endregion
    }
}
