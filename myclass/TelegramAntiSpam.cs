using System;
using System.Collections.Generic;
using System.Text;

namespace myclass
{

    public static partial class UtilityTelegram
    {
        private static Dictionary<long, DateTime> UsersLastMessage = new Dictionary<long, DateTime>();
        private static Dictionary<long, DateTime> UsersLastMessage2 = new Dictionary<long, DateTime>();
        private static Dictionary<long, DateTime> UsersLastMessage3 = new Dictionary<long, DateTime>();
        private static Dictionary<long, DateTime> UsersLastMessage4 = new Dictionary<long, DateTime>();
        private static Dictionary<long, DateTime> UsersLastMessage5 = new Dictionary<long, DateTime>();
        //  private static int ToWaitSeconds = 5;
        public static StringBuilder SpammChack(Telegram.Bot.Types.Message message, long toWaitSeconds)
        {
            StringBuilder result = new StringBuilder();
            long senderId = message.From!.Id;
            if (UsersLastMessage.TryGetValue(senderId, out DateTime lastMessage))
            {
                if ((message.Date - lastMessage).TotalSeconds < toWaitSeconds)
                {
                    result.Append($"{toWaitSeconds - (message.Date - lastMessage).TotalSeconds}");
                }
                else
                {
                    UsersLastMessage[senderId] = message.Date;
                    result.Append("ok");
                }

            }
            else
            {
                UsersLastMessage.Add(senderId, message.Date);
                result.Append("ok");
            }

            return result;
        }

        public static StringBuilder SpammChack2(Telegram.Bot.Types.CallbackQuery message, long toWaitSeconds)
        {
            StringBuilder result = new StringBuilder();
            long senderId = message.Message!.Chat!.Id;
            if (UsersLastMessage2.TryGetValue(senderId, out DateTime lastMessage))
            {
                if ((DateTime.Now - lastMessage).Seconds < toWaitSeconds)
                {
                    result.Append($"{toWaitSeconds - (DateTime.Now - lastMessage).Seconds}");
                }
                else
                {
                    UsersLastMessage2[senderId] = DateTime.Now;
                    result.Append("ok");
                }

            }
            else
            {
                UsersLastMessage2.Add(senderId, DateTime.Now);
                result.Append("ok");
            }

            return result;
        }
        //shisheget file
        public static StringBuilder SpammChack3(Telegram.Bot.Types.CallbackQuery message, long toWaitSeconds)
        {
            StringBuilder result = new StringBuilder();
            long senderId = message.Message!.Chat!.Id;
            if (UsersLastMessage3.TryGetValue(senderId, out DateTime lastMessage))
            {
                if ((DateTime.Now - lastMessage).Seconds < toWaitSeconds)
                {
                    result.Append($"{toWaitSeconds - (DateTime.Now - lastMessage).Seconds}");
                }
                else
                {
                    UsersLastMessage3[senderId] = DateTime.Now;
                    result.Append("ok");
                }

            }
            else
            {
                UsersLastMessage3.Add(senderId, DateTime.Now);
                result.Append("ok");
            }

            return result;
        }
        //zip
        public static StringBuilder SpammChack4(Telegram.Bot.Types.CallbackQuery message, long toWaitSeconds)
        {
            StringBuilder result = new StringBuilder();
            long senderId = message.Message!.Chat!.Id;
            if (UsersLastMessage4.TryGetValue(senderId, out DateTime lastMessage))
            {
                if ((DateTime.Now - lastMessage).Seconds < toWaitSeconds)
                {
                    result.Append($"{toWaitSeconds - (DateTime.Now - lastMessage).Seconds}");
                }
                else
                {
                    UsersLastMessage4[senderId] = DateTime.Now;
                    result.Append("ok");
                }

            }
            else
            {
                UsersLastMessage4.Add(senderId, DateTime.Now);
                result.Append("ok");
            }

            return result;
        }
        //unzip
        public static StringBuilder SpammChack5(Telegram.Bot.Types.CallbackQuery message, long toWaitSeconds)
        {
            StringBuilder result = new StringBuilder();
            long senderId = message.Message!.Chat!.Id;
            if (UsersLastMessage5.TryGetValue(senderId, out DateTime lastMessage))
            {
                if ((DateTime.Now - lastMessage).Seconds < toWaitSeconds)
                {
                    result.Append($"{toWaitSeconds - (DateTime.Now - lastMessage).Seconds}");
                }
                else
                {
                    UsersLastMessage5[senderId] = DateTime.Now;
                    result.Append("ok");
                }

            }
            else
            {
                UsersLastMessage5.Add(senderId, DateTime.Now);
                result.Append("ok");
            }

            return result;
        }
    }

}