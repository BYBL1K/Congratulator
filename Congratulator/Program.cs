using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Congratulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // !!Параметры подключения к БД, поменять под свой сервер
            string connStr = "server=localhost;user=root;database=peopledates;password=root";
            MySqlConnection conn = new MySqlConnection(connStr);
            // Создание подключения к БД
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("Не подключилась БД");
            }


            PersonDate personToDelete;
            PersonDate[] tempMassForSort;
            int countOfRecords = getCountRows();
            PersonDate[] massPeople = new PersonDate[countOfRecords];
            string commandMenu, commandDopMenu;
            bool boolForMenu, boolForMenuDop;


            // Получение количества записей в БД
            int getCountRows()
            {
                int count = 0;
                string sql = "SELECT * FROM person";
                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    count++;
                }
                reader.Close();
                return count;
            }
            // Получения массива объектов
            void getPersonFromBD()
            {
                int i = 0;
                string sql = "SELECT * FROM person";
                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    massPeople[i] = new PersonDate();
                    massPeople[i].Surname = reader[1].ToString();
                    massPeople[i].Date = DateTime.Parse(reader[2].ToString());
                    i++;
                }
                if (i==0)
                    Console.WriteLine("В базе данных отсутствуют записи, введите новые");
                reader.Close();
            }


            // Функция для ввода нового именинника
            void newPersonDate()
            {
                int count = massPeople.Length;
                Array.Resize(ref massPeople, (massPeople.Length + 1));
                massPeople[count] = new PersonDate();
                Console.Write("Введите имя именинника: ");
                massPeople[count].Surname = Console.ReadLine();
                massPeople[count].Date = inputDoB();
                Console.Clear();
                Console.WriteLine("Поздравляем, вы успешно добавили нового именинника!");
            }
            // Функция для удаления именинника
            PersonDate deletePersonDate()
            {
                string nameToDelete;
                DateTime dateToDelete;
                int num = 0;
                PersonDate tempPerson = new PersonDate();
                for (int l = 0; l < massPeople.Length; l++)
                {
                    massPeople[l].Print();
                }
                Console.Write("Введите имя того, кого хотите удалить: ");
                nameToDelete = Console.ReadLine();
                dateToDelete = inputDoB();
                for (int i = 0; i < massPeople.Length; i++)
                {
                    if (massPeople[i].Surname == nameToDelete && massPeople[i].Date == dateToDelete)
                    {
                        num = i;
                        tempPerson = massPeople[num];
                    }
                }
                List<PersonDate> listOfPerson = new List<PersonDate>(massPeople);
                listOfPerson.RemoveAt(listOfPerson.IndexOf(massPeople[num]));
                massPeople = listOfPerson.ToArray();
                Console.Clear();
                Console.WriteLine("Вы успешно удалили именинника");
                return tempPerson;
            }
            // Функция для изменения именинника
            int changePersonDate()
            {
                int numToChange = 0;
                Console.WriteLine("Нужно ввести отдельно имя и отдельно дату того, кого хотите изменить");
                Console.Write("Введите имя того, кого хотите изменить: ");
                string name = Console.ReadLine();
                DateTime dateChange = inputDoB();
                Console.Write("Введите новое имя: ");
                string newName = Console.ReadLine();
                DateTime newDate = inputDoB();
                for (int i = 0; i < massPeople.Length; i++)
                {
                    if (massPeople[i].Surname == name && massPeople[i].Date == dateChange)
                    {
                        numToChange++;
                        massPeople[i].Surname = newName;
                        massPeople[i].Date = newDate;
                    }
                }
                if (numToChange == 0)
                    Console.WriteLine("Такой именинник не найден");
                else
                    Console.WriteLine("Изменение успешно внесено");
                return numToChange;
            }


            // Сортировка по датам относительно даты сегодняшнего дня
            void sortByDate(PersonDate[] massOfPeople)
            {
                PersonDate[] tempMass = new PersonDate[1];
                tempMass[0] = new PersonDate();
                int tempDate1, tempDate2;
                for (int i = 1; i < massOfPeople.Length; i++)
                {
                    for (int j = 0; j < (massOfPeople.Length - i); j++)
                    {
                        tempDate1 = howDaysToB(massOfPeople[j].Date);
                        tempDate2 = howDaysToB(massOfPeople[j + 1].Date);
                        if (tempDate1 > tempDate2)
                        {
                            tempMass[0] = massOfPeople[j];
                            massOfPeople[j] = massOfPeople[j + 1];
                            massOfPeople[j + 1] = tempMass[0];
                        }
                    }
                }
            }
            // Функция ввода даты дня рождения с консоли
            DateTime inputDoB()
            {
                DateTime dob;
                string input;

                do
                {
                    Console.WriteLine("Введите дату рождения в формате дд.мм.гггг (день.месяц.год):");
                    input = Console.ReadLine();
                }
                while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, DateTimeStyles.None, out dob));

                return dob;
            }
            // Функция вычисления остатка дней до дня рождения
            int howDaysToB(DateTime date)
            {
                int result;
                TimeSpan dateDays;
                int year = DateTime.Today.Year - date.Year;
                if (date.Month < DateTime.Today.Month)
                {
                    year++;
                    date = date.AddYears(year);
                    dateDays = date - DateTime.Today;
                }
                else if (date.Month == DateTime.Today.Month)
                {
                    if (date.Day < DateTime.Today.Day)
                    {
                        year++;
                        date = date.AddYears(year);
                        dateDays = date - DateTime.Today;
                    }
                    else
                    {
                        date = date.AddYears(year);
                        dateDays = date - DateTime.Today;
                    }
                }
                else
                {
                    date = date.AddYears(year);
                    dateDays = date - DateTime.Today;
                }
                result = dateDays.Days;
                return result;
            }

            // Вывод сегодняшней даты
            void PrintDateToday()
            {
                Console.Write("Сегодня: " + DateTime.Today.ToString("d"));
                Console.WriteLine();
            }
            // Печать просроченных
            void PrintLaterDates(PersonDate[] massOfPeople)
            {
                string dateNewYear = "01.01." + (DateTime.Now.Year + 1).ToString();
                DateTime tempDate;
                int countOfLaters = 0;
                DateTime.TryParseExact(dateNewYear,"dd.MM.yyyy", null, DateTimeStyles.None , out tempDate);
                TimeSpan time;
                time = tempDate - DateTime.Now;
                Console.WriteLine("Вы пропустили в этом году:");
                for (int i = 0; i < massOfPeople.Length; i++)
                {

                    if (howDaysToB(massOfPeople[i].Date) >= time.Days)
                    {
                        massOfPeople[i].Print();
                        countOfLaters++;
                    }

                }
                if (countOfLaters == 0)
                    Console.WriteLine("В этом году еще ни у кого не было дня рождения!");
            }
            // Вывод ближайших именинников
            void PrintNextBirthday(PersonDate[] personDates)
            {
                int countTodayBitrhday = 0;
                int countTwoWeeks = 0;
                int numberElementmassTwoWeeks = 0;
                Console.Write("Именинники на сегодня:");
                for (int i = 0; i < personDates.Length; i++)
                {
                    if (howDaysToB(personDates[i].Date) == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(personDates[i].Surname);
                        countTodayBitrhday++;
                    }
                }
                if (countTodayBitrhday == 0)
                {
                    Console.WriteLine(" Сегодня нет именинников =(");
                }

                Console.WriteLine("Ближайшие именинники (2 недели):");

                for (int j = 0; j < personDates.Length; j++)
                {
                    if (howDaysToB(massPeople[j].Date) < 14 && howDaysToB(massPeople[j].Date) != 0)
                        countTwoWeeks++;
                }
                PersonDate[] tempMassDates = new PersonDate[countTwoWeeks];

                for (int n = 0; n < personDates.Length; n++)
                {
                    if (howDaysToB(massPeople[n].Date) < 14 && howDaysToB(massPeople[n].Date) != 0)
                    {
                        tempMassDates[numberElementmassTwoWeeks] = massPeople[n];
                        numberElementmassTwoWeeks++;
                    }
                }
                sortByDate(tempMassDates);

                for (int l = 0; l < tempMassDates.Length; l++)
                {
                    tempMassDates[l].Print();
                }

                if (countTwoWeeks == 0)
                {
                    Console.WriteLine("В ближайшие две недели нет именинников");
                }
                Console.WriteLine();
            }
            // Вывод всех именинников
            void PrintAllBithday(PersonDate[] personDates)
            {
                Console.WriteLine("Все дни рождения в вашей книге:");
                for (int i = 0; i < personDates.Length; i++)
                {
                    personDates[i].Print();
                }
            }



            // Вывод меню верхнего уровня
            void PrintMenuTop()
            {
                Console.WriteLine("Меню. Выберите пункт");
                Console.WriteLine("1. Отобразить весь список именинников");
                Console.WriteLine("2. Отобразить список сегодняшних и ближайших именинников");
                Console.WriteLine("3. Добавить запись о дне рождения");
                Console.WriteLine("4. Удалить запись о дне рождения");
                Console.WriteLine("5. Редактировать запись о дне рождения");
                Console.WriteLine("0. Выход из программы");
            }
            // Вывод меню дополнительных возможностей
            void PrintMenuDop()
            {
                Console.WriteLine("ПодМеню. Выберите пункт");
                Console.WriteLine("1. Отсортировать и вывести");
                Console.WriteLine("2. Выделить сегодняшние");
                Console.WriteLine("3. Вывести просроченные в этом году");
                Console.WriteLine("0. Выйти на уровень выше");
            }
            

            // Начало работы
            getPersonFromBD();
            if (countOfRecords > 0)
            {
                PrintDateToday();
                PrintNextBirthday(massPeople);
            }
            // Вывод меню
            do
            {
                boolForMenu = true;
                PrintMenuTop();
                commandMenu = Console.ReadLine();

                switch (commandMenu)
                {
                    case "1":
                        {
                            tempMassForSort = massPeople;
                            Console.Clear();
                            PrintDateToday();
                            PrintAllBithday(massPeople);
                            Console.WriteLine();
                            do
                            {
                                boolForMenuDop = true;
                                PrintMenuDop();
                                commandDopMenu = Console.ReadLine();
                                switch (commandDopMenu)
                                {
                                    case "1":{
                                            Console.Clear();
                                            sortByDate(tempMassForSort);
                                            PrintDateToday();
                                            for (int i = 0; i < tempMassForSort.Length; i++)
                                            {
                                                tempMassForSort[i].Print();
                                            }
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "2":{
                                            Console.Clear();
                                            PrintDateToday();
                                            PrintNextBirthday(tempMassForSort);
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "3": {
                                            Console.Clear();
                                            PrintDateToday();
                                            PrintLaterDates(tempMassForSort);
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "0":
                                        boolForMenuDop = false;
                                        Console.Clear();
                                        break;
                                    default:
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Вы ввели неверное значение");
                                        }
                                        break;
                                }

                            } while (boolForMenuDop);
                            
                        }
                        break;
                    case "2":
                        {
                            Console.Clear();
                            PrintDateToday();
                            PrintNextBirthday(massPeople);
                        }
                        break;
                    case "3":{
                            Console.Clear();
                            PrintAllBithday(massPeople);
                            newPersonDate();
                            massPeople.Last().WriteToDB(conn);
                        }
                        break;
                    case "4":{
                            Console.Clear();
                            personToDelete = deletePersonDate();
                            personToDelete.DeleteFromBD(conn);
                        }
                        break;
                    case "5":{
                            Console.Clear();
                            PrintAllBithday(massPeople);
                            int numToChange = changePersonDate();
                            massPeople[numToChange].ChangeInBD(conn);
                        }
                        break;
                    case "0":
                        boolForMenu = false;
                        break;
                    default:
                        {
                            Console.Clear();
                            Console.WriteLine("Вы ввели неверное значение");
                        }
                        break;
                }

            }
            while (boolForMenu);

            // Закрытие соединения с БД
            conn.Close();
        }
    }
}
