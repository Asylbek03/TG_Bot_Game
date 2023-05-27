using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace TG_BOT
{
	public class Function
	{
		    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

		struct BotUpdate
		{
			public string text;
			public long id;
			public string? username;
		
		}
		class Program
		{
			static TelegramBotClient Bot = new TelegramBotClient("6197009361:AAENWL8Bjn18gZPhdyUCOcmzPKHpHWLDIlU");

			static string fileName = "updates.json";

			const int arrSize = 89;
			static String[,] NameOfCitiesMatrix = new String[arrSize,2] {{"Астана", "0"}, 
																		{"Алмата", "0"}, 
																		{"Абай", "0"},
																		{"Акколь", "0"},
																		{"Аксай", "0"},
																		{"Аксу", "0"},
																		{"Актау", "0"},
																		{"Актобе", "0"},
																		{"Алга", "0"},
																		{"Алтай", "0"},
																		{"Аральск", "0"},
																		{"Аркалык", "0"},
																		{"Арыс", "0"},
																		{"Атбасар", "0"},
																		{"Атырау", "0"},
																		{"Аягоз", "0"},
																		{"Байконур", "0"},
																		{"Балхаш", "0"},
																		{"Булаево", "0"},
																		{"Державинск", "0"},
																		{"Ерейментау", "0"},
																		{"Есик", "0"},
																		{"Есиль", "0"},
																		{"Жанаозен", "0"},
																		{"Жанатас", "0"},
																		{"Жаркент", "0"},
																		{"Жезказган", "0"},
																		{"Жем", "0"},
																		{"Жетысай", "0"},
																		{"Житикара", "0"},
																		{"Зайса", "0"},
																		{"Казалинск", "0"},
																		{"Кандыагаш", "0"},
																		{"Караганда", "0"},
																		{"Каражал", "0"},
																		{"Каратау", "0"},
																		{"Каркаралинск", "0"},
																		{"Каскелен", "0"},
																		{"Кентау", "0"},
																		{"Кокшетау", "0"},
																		{"Конаев", "0"},
																		{"Костанай", "0"},
																		{"Косшы", "0"},
																		{"Кульсары", "0"},
																		{"Курчатов", "0"},
																		{"Кызылорда", "0"},
																		{"Ленгер", "0"},
																		{"Лисаковск", "0"},
																		{"Макинск", "0"},
																		{"Мамлютка","0"},
																		{"Павлодар","0"},
																		{"Петропавловск","0"},
																		{"Приозёрск","0"},
																		{"Риддер","0"},
																		{"Рудный","0"},
																		{"Сарань","0"},
																		{"Сарканд","0"},
																		{"Сарыагаш","0"},
																		{"Сатпаев","0"},
																		{"Семей","0"},
																		{"Сергеевка","0"},
																		{"Серебрянск","0"},
																		{"Степногорск","0"},
																		{"Степняк","0"},
																		{"Тайынша","0"},
																		{"Талгар","0"},
																		{"Талдыкорган","0"},
																		{"Тараз","0"},
																		{"Текели","0"},
																		{"Темир","0"},
																		{"Темиртау","0"},
																		{"Тобыл","0"},
																		{"Туркестан","0"},
																		{"Уральск","0"},
																		{"Усть-Каменогорск","0"},
																		{"Ушарал","0"},
																		{"Уштобе","0"},
																		{"Форт-Шевченко","0"},
																		{"Хромтау","0"},
																		{"Шалкар","0"},
																		{"Шар","0"},
																		{"Шардара","0"},
																		{"Шахтинск","0"},
																		{"Шемонаиха","0"},
																		{"Шу","0"},
																		{"Шымкент","0"},
																		{"Щучинск","0"},
																		{"Экибастуз","0"},
																		{"Эмба","0"}
																		};

			static bool StartGame = false;
			static bool IsLocated=false;
			static bool BotWordLocated=false;
			static Timer timer;	
			static bool GameOver=false;
			static bool FirstWord = true;
			static String UserAnswer,UserAnswerLower;
			static String botCityName = "";
			static bool cityFound = false; 
			static int UserScore=0, BotScore=0;


			static List<BotUpdate> botUpdates = new List<BotUpdate>();
			static void Main(string[] args)
			{
				try
				{
					var botUpdatesString = System.IO.File.ReadAllText(fileName);

					botUpdates = JsonConvert.DeserializeObject<List<BotUpdate>>(botUpdatesString) ?? botUpdates;
				}
				catch(Exception ex)
				{
					Console.WriteLine($"Error reading or deserializing {ex}");
				}

				var receiverOptions = new ReceiverOptions
				{
					AllowedUpdates = new UpdateType[]
					{
						UpdateType.Message,
						UpdateType.EditedMessage,
					}
				};

				Bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
				
				Console.ReadLine();
			}

			private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
			{
				throw new NotImplementedException();
			}

			private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
			{
				if(update.Type == UpdateType.Message)
				{
					if(update.Message.Type == MessageType.Text)
					{
						if(StartGame == true){
							UserAnswer = update.Message.Text;
							if(FirstWord){
								for (int i = 0; i < arrSize; i++)
								{
									if (NameOfCitiesMatrix[i, 0] == UserAnswer)
									{
										if (NameOfCitiesMatrix[i, 1] == "0")
										{
											NameOfCitiesMatrix[i, 1] = "1";
											cityFound = true;
											FirstWord = false;
											UserScore++;
											break;
										}
										else
										{
											await bot.SendTextMessageAsync(
												chatId: update.Message.Chat.Id,
												text: "Город занят");
												timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
											cityFound = true;
											return;
										}
									}
								}
								
								if (!cityFound)
								{
									await bot.SendTextMessageAsync(
										chatId: update.Message.Chat.Id,
										text: "Город не найден");
										timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
										return;
								}
								for(int i = 0; i<arrSize; i++)
								{
									botCityName=NameOfCitiesMatrix[i,0].ToLower();
									if(UserAnswer[UserAnswer.Length-1]==botCityName[0]){
										if (NameOfCitiesMatrix[i, 1] == "0")
											{
												await bot.SendTextMessageAsync(
												chatId: update.Message.Chat.Id,
												text: NameOfCitiesMatrix[i,0]);
												NameOfCitiesMatrix[i,1]="1";
												BotScore++;
												timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
												return;
											}
											else continue;
									}

								}
							}
							else {
								UserAnswerLower=update.Message.Text.ToLower();
								UserAnswer=update.Message.Text;
								timer.Dispose();
									if(UserAnswerLower[0] == botCityName[botCityName.Length-1]){
										for (int i = 0; i < arrSize; i++)
										{
											if (NameOfCitiesMatrix[i, 0] == UserAnswer){
												IsLocated=true;
												break;
											}
											else IsLocated=false;
										}
										if(!IsLocated){
											await bot.SendTextMessageAsync(
													chatId: update.Message.Chat.Id,
													text: "Город не найден");
												cityFound = true;
												timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
												return;
										}
										for (int i = 0; i < arrSize; i++)
										{
											if (NameOfCitiesMatrix[i, 0] == UserAnswer)
											{
												if (NameOfCitiesMatrix[i, 1] == "0")
												{
													NameOfCitiesMatrix[i, 1] = "1";
													cityFound = true;
													FirstWord = false;
													UserScore++;
													break;
												}
												else
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Город занят");
														timer = new Timer(async _ =>
														{
															await bot.SendTextMessageAsync(
																chatId: update.Message.Chat.Id,
																text: "Прошло 10 секунд!\n"+"Вы проиграли!");
															StartGame=false;
														}, null, 10000, Timeout.Infinite);
													cityFound = true;
													return;
												}
											}
										}
										
										if (!cityFound)
										{
											await bot.SendTextMessageAsync(
												chatId: update.Message.Chat.Id,
												text: "Город не найден");
												timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
												return;
										}
									}
									else{
										await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Первая буква не совпадает");
												timer = new Timer(async _ =>
												{
													await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: "Прошло 10 секунд!\n"+"Вы проиграли!");
													StartGame=false;
												}, null, 10000, Timeout.Infinite);
										return;
									}
									for(int i = 0; i<arrSize; i++)
									{
										botCityName=NameOfCitiesMatrix[i,0].ToLower();
										if(UserAnswer[UserAnswer.Length-1]==botCityName[0]){
											BotWordLocated=true;
											break;
										}
										else BotWordLocated=false;
									}
									if(!BotWordLocated){
										await bot.SendTextMessageAsync(
											chatId: update.Message.Chat.Id,
											text: "Я не нашёл подходящий город.");
										StartGame=false;
										if(UserScore>BotScore)
										await bot.SendTextMessageAsync(
											chatId: update.Message.Chat.Id,
											text: "Поздравляю вы выйграли!\n"+"Количество набранных очков: "+UserScore+".\n"+"Количество набранных очков ботом: "+BotScore+".");
										else if(UserScore==BotScore)
										await bot.SendTextMessageAsync(
											chatId: update.Message.Chat.Id,
											text: "Поздравляю победила дружба!\n"+"Количество набранных очков: "+UserScore+".\n"+"Количество набранных очков ботом: "+BotScore+".");
										else 
										await bot.SendTextMessageAsync(
											chatId: update.Message.Chat.Id,
											text: "К сожалению вы проиграли, выйграл бот!\n"+"Количество набранных очков: "+UserScore+".\n"+"Количество набранных очков ботом: "+BotScore+".");
										return;
									}
									for(int i = 0; i<arrSize; i++)
									{
										botCityName=NameOfCitiesMatrix[i,0].ToLower();
										if(UserAnswer[UserAnswer.Length-1]==botCityName[0]){
											if (NameOfCitiesMatrix[i, 1] == "0")
													{
														await bot.SendTextMessageAsync(
														chatId: update.Message.Chat.Id,
														text: NameOfCitiesMatrix[i,0]);
														NameOfCitiesMatrix[i,1]="1";
														BotScore++;
														timer = new Timer(async _ =>
														{
															await bot.SendTextMessageAsync(
																chatId: update.Message.Chat.Id,
																text: "Прошло 10 секунд!\n"+"Вы проиграли!");
															StartGame=false;
														}, null, 10000, Timeout.Infinite);
														return;
													}
													else continue;
										}
									}
							}
						}
						if (update.Message.Text.ToLower() == "/start")
						{
						await bot.SendTextMessageAsync(
							chatId: update.Message.Chat.Id,
							text: "Привет!\n\n" + "Напишите команду /game чтобы сыграть в города.\n\n"+"Правила игры в города: каждый участник в свою очередь называет реально существующий город Казахстана, название которого начинается на ту букву, которой оканчивается название предыдущего участника. "+
							"Введите название города в течении 10 секунд.");

						return;
						}
						if (update.Message.Text.ToLower() == "/game")
						{
						await bot.SendTextMessageAsync(
							chatId: update.Message.Chat.Id,
							text: "Игра начинается!\n" + "Напишите название города");
							StartGame = true;
							for(int i=0;i<arrSize;i++){
								NameOfCitiesMatrix[i,1]="0";
							}
						return;
						}
						
						var _botUpdate = new BotUpdate
						{
							text = update.Message.Text,
							id = update.Message.Chat.Id,
							username = update.Message.Chat.Username
						};

						botUpdates.Add(_botUpdate);

						var botUpdatesString = JsonConvert.SerializeObject(botUpdates);

						System.IO.File.WriteAllText(fileName, botUpdatesString);

						
					}
				}
			}
		}
	}
}