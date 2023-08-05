using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Controllers
{
    [Route("bot/start")]
    [ApiController]
    public class ConfigSender : ControllerBase
    {
        public static int tekrar;
        static string GetDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\USERS";
        public static TelegramBotClient Bot;
        static Task<User> botidme;
        static string botjson;
        static HashSet<long> userIds = new HashSet<long>();

        private readonly IConfiguration _configuration;
        private static string _adminId;

        public ConfigSender(IConfiguration configuration)
        {
            _configuration = configuration;
            Bot = new TelegramBotClient(_configuration["TelegramSettings:BotToken"]);
            botidme = Bot.GetMeAsync();
            botjson = JsonConvert.SerializeObject(botidme.Result, Formatting.Indented);
            _adminId = _configuration["TelegramSettings:AdminID"];
        }
        [Obsolete]
        [HttpGet]
        public IActionResult Info()
        {
            try
            {
                tekrar++;
                if (tekrar >= 2)
                {
                    try
                    {
                        return new OkObjectResult(botjson);
                    }
                    catch (Exception)
                    {
                        return new OkObjectResult("Error getting bot information, reload the page");
                    }
                }

                botrunner();
                return new OkObjectResult(botjson);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it somehow
                return new StatusCodeResult(500);
            }
        }

        [Obsolete]
        public static Task botrunner()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                        UpdateType.Message,
                        UpdateType.EditedMessage,
                        UpdateType.CallbackQuery,
                        UpdateType.ChosenInlineResult,
                        UpdateType.InlineQuery,
                        UpdateType.PollAnswer,
                        UpdateType.Unknown,
                        UpdateType.ChatMember,
                }
            };

            Bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);

            return Task.CompletedTask;
        }

        private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return Task.CompletedTask;
        }

        [Obsolete("Obsolete")]
        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    if (message != null && message.From != null)
                    {
                        var totalWait = SpammCheck.Check(update);

                        if (totalWait != "ok")
                        {
                            await Bot.SendTextMessageAsync(
                                update.Message.From!.Id,
                                $"*یه‌کم آروم‌تر \ud83d\ude42\n بعد از {totalWait} ثانیه می‌تونی مجددا درخواست بفرستی.*",
                                replyToMessageId: update.Message.MessageId,
                                parseMode: ParseMode.Markdown,
                                cancellationToken: arg3);
                            return;
                        }
                        userIds.Add(message.From.Id);
                        await SaveUserIds();

                        // Check for commands and send files
                        if (message.Text != null)
                        {
                            if (message.Text == "/start")
                            {
                                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                {
                                new[] // first row
                                {
                                    InlineKeyboardButton.WithCallbackData("دریافت کانفیگ \ud83d\udd25", "/get_config"),
                                },
                                new[] // first row
                                {
                                    InlineKeyboardButton.WithCallbackData("آموزش اتصال \ud83d\udca1", "/get_instruction"),
                                }
                            });

                                await bot.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: $"سلام به ربات {botidme.Result.FirstName} خوش آمدید \ud83d\udc4b" + "\n\n" +
                                          "یک گزینه را انتخاب کنید:",
                                    replyMarkup: inlineKeyboard,
                                    replyToMessageId: message.MessageId, cancellationToken: arg3);
                            }
                        }
                    }
                }

                if (update.Type == UpdateType.CallbackQuery)
                {
                      var totalWait = SpammCheck.Check(update);

                            if (totalWait != "ok")
                            {
                                await Bot.AnswerCallbackQueryAsync(
                                    update.CallbackQuery.Id,
                                    $"یه‌کم آروم‌تر \ud83d\ude42\n بعد از {totalWait} ثانیه می‌تونی مجددا درخواست بفرستی.",
                                    true,
                                    cancellationToken: arg3
                                );

                                return;
                            }
                    var callbackQuery = update.CallbackQuery;
                    string directoryPath = null;

                    switch (callbackQuery.Data)
                    {
                        case "/get_instruction":
                            directoryPath = Path.Combine(GetDirectory, "learn");
                            break;
                        case "/get_config":
                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new[] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("کانفیگ NapsternetV برای اندروید", "/get_inpv_files"),
                        },
                        new[] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("کانفیگ NapsternetV برای IOS", "/get_npv4_files"),
                        },
                        new[] // third row
                        {
                            InlineKeyboardButton.WithCallbackData("کانفیگ DarkTunnel", "/get_dark_files"),
                        },
                        new[] // fourth row
                        {
                            InlineKeyboardButton.WithCallbackData("دریافت کانفیگ V2RAY", "/send_config_text"),
                        },
                    });

                            await bot.SendTextMessageAsync(
                                chatId: callbackQuery.Message.Chat.Id,
                                text: "یک گزینه را انتخاب کنید:",
                                replyMarkup: inlineKeyboard,
                                replyToMessageId: callbackQuery.Message.MessageId, cancellationToken: arg3);
                            break;
                        case "/get_inpv_files":
                            directoryPath = Path.Combine(GetDirectory, "inpv");
                            break;
                        case "/get_npv4_files":
                            directoryPath = Path.Combine(GetDirectory, "npv4");
                            break;
                        case "/get_dark_files":
                            directoryPath = Path.Combine(GetDirectory, "dark");
                            break;
                        case "/send_config_text":
                            await SendConfigText(bot, callbackQuery.Message.Chat.Id, callbackQuery.Id, callbackQuery.Message.MessageId);
                            break;
                    }

                    if (directoryPath != null)
                    {
                        await SendFilesFromDirectory(bot, directoryPath, callbackQuery.Message.Chat.Id, callbackQuery.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it somehow
                await Bot.SendTextMessageAsync(_adminId, $"Error in Update: {ex.Message}", cancellationToken: arg3);
            }
        }

        private static async Task SendConfigText(ITelegramBotClient bot, long chatId, string callbackQueryId, int messageId)
        {
            try
            {
                var configTextFilePath = Path.Combine(GetDirectory, "configtxt", "config.txt");

                if (!System.IO.File.Exists(configTextFilePath))
                {
                    await bot.AnswerCallbackQueryAsync(callbackQueryId, "فایل مورد نظر موجود نیست.");
                    return;
                }

                var configText = await System.IO.File.ReadAllTextAsync(configTextFilePath);
                await bot.SendTextMessageAsync(chatId, configText, replyToMessageId: messageId, parseMode: ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it somehow
                await Bot.SendTextMessageAsync(_adminId, $"Error in SendConfigText: {ex.Message}");
            }
        }
        private static Dictionary<string, string> fileIds = new Dictionary<string, string>();

        private static async Task SendFilesFromDirectory(ITelegramBotClient bot, string directoryPath, long chatId, string callbackQueryId)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    await bot.AnswerCallbackQueryAsync(callbackQueryId, "کانفیگ مورد نظر فعلا موجود نیست از کانفیگ های دیگر استفاده کنید\u2764\ufe0f\u200d\ud83d\udd25");
                    return;
                }

                var filePaths = Directory.GetFiles(directoryPath);
                if (filePaths.Length == 0)
                {
                    await bot.AnswerCallbackQueryAsync(callbackQueryId, "کانفیگ مورد نظر فعلا موجود نیست از کانفیگ های دیگر استفاده کنید\u2764\ufe0f\u200d\ud83d\udd25");
                    return;
                }

                foreach (var filePath in filePaths)
                {
                    var caption = Path.GetFileNameWithoutExtension(filePath); // Use the file name without extension as caption

                    if (fileIds.ContainsKey(filePath))
                    {
                        // If the file has been uploaded before, just resend it.
                        var fileId = fileIds[filePath];

                        if (Path.GetExtension(filePath) == ".mp4")
                        {
                            await bot.SendVideoAsync(chatId, new InputFileId(fileId), caption: caption);
                        }
                        else
                        {
                            await bot.SendDocumentAsync(chatId, new InputFileId(fileId), caption: caption);
                        }

                        continue;
                    }

                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var fileName = Path.GetFileName(filePath);

                    if (Path.GetExtension(filePath) == ".mp4")
                    {
                        var message = await bot.SendVideoAsync(chatId, new InputFileStream(fileStream, fileName), caption: caption);
                        fileIds[filePath] = message.Video.FileId;
                    }
                    else
                    {
                        var message = await bot.SendDocumentAsync(chatId, new InputFileStream(fileStream, fileName), caption: caption);
                        fileIds[filePath] = message.Document.FileId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it somehow
                await Bot.SendTextMessageAsync(_adminId, $"Error in SendFilesFromDirectory: {ex.Message}");
            }
        }

        public static async Task SaveUserIds()
        {
            try
            {
                await System.IO.File.WriteAllLinesAsync(GetDirectory + "\\users.txt", userIds.Select(id => id.ToString()));
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(GetDirectory);
                await System.IO.File.WriteAllLinesAsync(GetDirectory + "\\users.txt", userIds.Select(id => id.ToString()));
            }
        }

        public static async void LoadUserIds()
        {
            try
            {
                // Create directories if they don't exist
                try
                {
                    Directory.CreateDirectory(Path.Combine(GetDirectory, "inpv"));
                    Directory.CreateDirectory(Path.Combine(GetDirectory, "npv4"));
                    Directory.CreateDirectory(Path.Combine(GetDirectory, "dark"));
                    Directory.CreateDirectory(Path.Combine(GetDirectory, "configtxt"));
                    Directory.CreateDirectory(Path.Combine(GetDirectory, "learn"));

                }
                catch (Exception ex)
                {
                    await Bot.SendTextMessageAsync(_adminId, $"Could not create directories: {ex.Message}");
                }

                try
                {
                    var lines = await System.IO.File.ReadAllLinesAsync(GetDirectory + "\\users.txt");
                    userIds = new HashSet<long>(lines.Select(long.Parse));
                }
                catch (FileNotFoundException)
                {
                    // اگر فایل وجود نداشت، HashSet را خالی بگذارید
                    userIds = new HashSet<long>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it somehow
                await Bot.SendTextMessageAsync(_adminId, $"Error in LoadUserIds: {ex.Message}");
            }
        }

    }
}