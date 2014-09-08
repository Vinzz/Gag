/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.Net.Mail;

namespace Gag
{
    public abstract class MailMan
    {
        public static void SendMail(GagData gData)
        {
            try
            {
                MailMessage Message = new MailMessage(gData.MailFrom,
                                                      gData.MailTo,
                                                      gData.MailSubject,
                                                      gData.MailText);

                SmtpClient mailClient = new SmtpClient(gData.MailServer);
                mailClient.Send(Message);
            }
            catch (System.Web.HttpException ehttp)
            {
                MessageBox.Show(ehttp.Message, "Mailing Error");
            }
        }
    }
}
