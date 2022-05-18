using SeatBookingService.Models.DTO;
using System;
using System.Drawing;
using System.IO;

namespace SeatBookingService.Utility
{
    public class GenerateTicket
    {
        public static Image DrawTicket(TicketDto ticket, Color textColor, Color backColor)
        {
            int SPACE = 145;
            int width = Convert.ToInt32(435);
            int height = Convert.ToInt32(465);
            string TType = "S";

            Brush textBrush = new SolidBrush(textColor);

            Font fBody = new Font("Lucida Console", 12, FontStyle.Bold);
            Font fBody1 = new Font("Lucida Console", 12, FontStyle.Regular);
            Font rs = new Font("Stencil", 20, FontStyle.Bold);
            Font fTType = new Font("", 150, FontStyle.Bold);

            //first, create a dummy bitmap just to get a graphics object  
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //free up the dummy image and old graphics object  
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size  
            img = new Bitmap(width, height);

            drawing = Graphics.FromImage(img);

            //paint the background  
            drawing.Clear(backColor);

            //draw border
            drawing.DrawRectangle(Pens.Black, 5, 5, 420, 450);

            //Draw Text
            drawing.DrawString("Bus Bintang Utara", rs, textBrush, 10, 50);

            drawing.DrawString("-------------------------------", fBody1, textBrush, 10, 120);

            drawing.DrawString("Tanggal : " + ticket.schedule_date.ToShortDateString(), fBody, textBrush, 10, SPACE);
            drawing.DrawString("Nomor Bus : " + ticket.no_bus, fBody, textBrush, 10, SPACE + 30);
            drawing.DrawString("Rute : " + ticket.origin + " - " + ticket.destination, fBody, textBrush, 10, SPACE + 60);
            drawing.DrawString("Harga (/tiket) : Rp. " + string.Format("{0:0,0}", ticket.price), fBody, textBrush, 10, SPACE + 90);
            drawing.DrawString("Total Harga : Rp. " + string.Format("{0:0,0}", ticket.total_price), fBody, textBrush, 10, SPACE + 120);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
