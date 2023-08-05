using System.Collections.Generic;
using System;
using Telegram.Bot.Types;

public class SpammCheck
{
    private static Dictionary<string, DateTime> data = new Dictionary<string, DateTime>();
    private static Dictionary<string, DateTime> banData = new Dictionary<string, DateTime>();
    private const int antifloodSeconds = 1; // The time window for counting the messages
    private const double banSeconds = 0.5; // The duration of the ban

    public static string Check(Update update)
    {
        try
        {
            long chatId;
            long userId;
            if (update.Message != null)
            {
                chatId = update.Message.Chat.Id;
                userId = update.Message.From.Id;
            }
            else if (update.CallbackQuery != null)
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                userId = update.CallbackQuery.From.Id;
            }
            else
            {
                return "invalid update";
            }

            var key = $"{chatId}:{userId}";

            if (banData.ContainsKey(key))
            {
                var elapsedSeconds = (DateTime.UtcNow - banData[key]).TotalSeconds;
                if (elapsedSeconds <= banSeconds)
                {
                    var remainingSeconds = Math.Ceiling(banSeconds - elapsedSeconds);
                    return $"{remainingSeconds}";
                }
                else
                {
                    banData.Remove(key);
                }
            }

            if (!data.ContainsKey(key))
            {
                data[key] = DateTime.UtcNow;
                return "ok";
            }

            var timeSpan = DateTime.UtcNow - data[key];
            if (timeSpan.TotalSeconds <= antifloodSeconds)
            {
                banData[key] = DateTime.UtcNow;
                return $"{banSeconds}";
            }

            data[key] = DateTime.UtcNow;
            return "ok";
        }
        catch (Exception ex)
        {
            // Here you can log the exception or rethrow it
            return $"Error: {ex.Message}";
        }
    }
}