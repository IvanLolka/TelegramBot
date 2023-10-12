using System.IO.Compression;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static bool Calculate = false;
    static int Proverka = 0;
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;
    static async Task Main()
    {

        _botClient = new TelegramBotClient("6443824650:AAFIHbkJXfuBj3rQ2njAC1XiktwSs0NpFHg"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                UpdateType.CallbackQuery // Inline кнопки
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {

            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                    {

                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;

                        // From - это от кого пришло сообщение
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;
                        switch (message.Type)
                        {
                            // Тут понятно, текстовый тип
                            case MessageType.Text:
                                {
                                    // тут обрабатываем команду /start, остальные аналогично
                                    if (message.Text == "/start" || message.Text == "Обратно")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Включить калькулятор")
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Мем"),
                                                    new KeyboardButton("Видео")
                                                }
                                            })
                                        {
                                            // автоматическое изменение размера клавиатуры, если не стоит true,
                                            // тогда клавиатура растягивается чуть ли не до луны,
                                            // проверить можете сами
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Команды\n" +
                                            "/calculator\n" +
                                            "/mem\n" +
                                            "/video", replyMarkup: replyKeyboard);
                                        return;
                                    }
                                    if (message.Text == "Включить калькулятор" || message.Text == "/calculator")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Выключить из калькулятора")
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Мем"),
                                                    new KeyboardButton("Видео")
                                                }
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        Calculate = true;
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Калькулятор включён\n" +
                                        "Пример ввода: \'а + б\', где \'а\' это первое число, \'б\' второе число и ввод знака(+=*/) через пробел\n" +
                                        "/offcalc", replyMarkup: replyKeyboard);
                                        return;
                                    }
                                    if (message.Text == "Выключить из калькулятора" || message.Text == "/offcalc")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Включить калькулятор")
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Мем"),
                                                    new KeyboardButton("Видео")
                                                }
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        Calculate = false;
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Калькулятор выключен", replyMarkup: replyKeyboard);
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Команды\n" +
                                            "/calculator\n" +
                                            "/mem\n" +
                                            "/video");
                                        return;
                                    }
                                    if (Calculate == true)
                                    {
                                        double answer = 0;
                                        string[] mess = message.Text.Split(" ");
                                        foreach (string item in mess)
                                        {
                                            if (item == "+")
                                            {
                                                answer = Convert.ToInt32(mess[0]) + Convert.ToInt32(mess[2]);
                                            }
                                            if (item == "-")
                                            {
                                                answer = Convert.ToInt32(mess[0]) - Convert.ToInt32(mess[2]);
                                            }
                                            if (item == "*")
                                            {
                                                answer = Convert.ToInt32(mess[0]) * Convert.ToInt32(mess[2]);
                                            }
                                            if (item == "/")
                                            {
                                                answer = Convert.ToDouble(mess[0]) / Convert.ToDouble(mess[2]);
                                            }
                                        }
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Вот ответ: " + Convert.ToString(answer));
                                        return;
                                    }
                                    if (message.Text == "Мем" || message.Text == "/mem")
                                    {
                                        Random rnd = new Random();
                                        int RndFoto = rnd.Next(1, 8);
                                        while (Proverka == RndFoto)
                                        {
                                            RndFoto = rnd.Next(1, 8);
                                        }
                                        var foto = "";
                                        if (RndFoto == 1)
                                        {
                                            foto = "https://cs9.pikabu.ru/post_img/2017/05/27/12/1495915841119994041.jpg";
                                        }
                                        if (RndFoto == 2)
                                        {
                                           foto = "https://s8.hostingkartinok.com/uploads/images/2020/12/82f4ff902411333dd82a9963fc7dce6f.jpg";
                                        }
                                        if (RndFoto == 3)
                                        {
                                           foto = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDLL4MKXnlE_EFOsnzRaOUZSo9sjRAnvgO2Q&usqp=CAU";
                                        }
                                        if (RndFoto == 4)
                                        {
                                            foto = "https://timestable.ru/photo/memes/1.jpg";
                                        }
                                        if (RndFoto == 5)
                                        {
                                            foto = "https://shutniks.com/wp-content/uploads/2020/03/0dbdbe8ab796edd5c71024622b6b66664459de67r1-452-413v2_hq.jpg";
                                        }
                                        if (RndFoto == 6)
                                        {
                                            foto = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTKQMetY5lgJY0c62nfRej8GTpljntF2piR4g&usqp=CAU";
                                        }
                                        if (RndFoto == 7)
                                        {
                                            foto = "https://sun9-5.userapi.com/impf/Dv4kQknu6H1O3PFqymTM_tgPqw2Le9ZU6x6bJw/Q6cjeHEtCts.jpg?size=376x604&quality=96&sign=4fb5ba752f00020df2edb681a8f551cf&type=album";
                                        }
                                        InputFileUrl inputOnlineFile = new(foto);
                                        await botClient.SendPhotoAsync(chat.Id, inputOnlineFile);
                                        Proverka = RndFoto;
                                        return;
                                    }
                                    if (message.Text == "Видео" || message.Text == "/video")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Проценты"),
                                                    new KeyboardButton("Сложение")
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Обратно")
                                                }
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "/percents\n" +
                                        "/sum", replyMarkup: replyKeyboard);
                                        return;
                                    }
                                    if (message.Text == "Проценты" || message.Text == "/percents")
                                    {
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "https://www.youtube.com/watch?v=OWLl8myTI7A");
                                        return;
                                    }
                                    if (message.Text == "Сложение" || message.Text == "/sum")
                                    {
                                        await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "https://www.youtube.com/watch?v=GK2-4eCmj2U");
                                        return;
                                    }
                                    return;
                                }

                            // Добавил default , чтобы показать вам разницу типов Message
                            default:
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Используй только текст!");
                                    return;
                                }
                        }

                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }


}
