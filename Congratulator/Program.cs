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
            // Создание подключения к БД
            string connStr = "server=localhost;user=root;database=peopledates;password=root";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("Не подключилась БД");
            }

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

            PersonDate tPerson;
            PersonDate[] tempMassSort;
            int c = getCountRows();
            PersonDate[] massPeople = new PersonDate[c];

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
                    massPeople[i].Id = Convert.ToInt32(reader[0]);
                    massPeople[i].Surname = reader[1].ToString();
                    massPeople[i].Date = DateTime.Parse(reader[2].ToString());
                    i++;
                }
                if (i==0)
                    Console.WriteLine("В базе данных отсутствуют записи, введите новые");
                reader.Close();
            }

            // Сортировка по датам
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
           
            // Печать просроченных
            void printLaterDates(PersonDate[] massOfPeople)
            {
                string dateNewYear = "01.01." + (DateTime.Now.Year + 1).ToString();
                DateTime tempDate;
                DateTime.TryParseExact(dateNewYear,"dd.MM.yyyy", null, DateTimeStyles.None , out tempDate);
                TimeSpan time;
                time = tempDate - DateTime.Now;
                Console.WriteLine("Вы пропустили в этом году:");
                for (int i = 0; i < massOfPeople.Length; i++)
                {

                    if (howDaysToB(massOfPeople[i].Date) >= time.Days)
                    {
                        massOfPeople[i].Print();
                    }

                }
            }
           
            // Функция для ввода нового именинника
            void newPersonDate()
            {
                Array.Resize(ref massPeople, (c + 1) );
                massPeople[c] = new PersonDate();
                massPeople[c].Id = c ;
                Console.Write("Введите имя именинника: ");
                massPeople[c].Surname = Console.ReadLine();
                massPeople[c].Date = inputDoB();
                c++;
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
                for (int i = 0; i < c; i++)
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
                c--;
                Console.Clear();
                Console.WriteLine("Вы успешно удалили именинника");
                return tempPerson;
            }
           
            // Функция для изменения именинника
            int changePersonDate()
            {
                int numToChange = 0;
                Console.Write("Введите имя того, кого хотите изменить: ");
                string name = Console.ReadLine();
                DateTime dateChange = inputDoB();
                Console.Write("Введите новое имя: ");
                string newName = Console.ReadLine();
                DateTime newDate = inputDoB();
                for (int i = 0; i < c; i++)
                {
                    if (massPeople[i].Surname == name && massPeople[i].Date == dateChange)
                    {
                        numToChange = i;
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
            
            // Вывод ближайших именинников
            void PrintNextBirthday(PersonDate[] personDates)
            {
                int i = 0;
                int j = 0;
                int k = 0;
                Console.Write("Именинники на сегодня:");
                for (int c1 = 0; c1 < personDates.Length; c1++)
                {
                    if (howDaysToB(personDates[c1].Date) == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(personDates[c1].Surname);
                        i++;
                    }
                }
                if (i == 0)
                {
                    Console.WriteLine(" Сегодня нет именинников =(");
                }

                Console.WriteLine("Ближайшие именинники (2 недели):");

                for (int c2 = 0; c2 < personDates.Length; c2++)
                {
                    if (howDaysToB(massPeople[c2].Date) < 14 && howDaysToB(massPeople[c2].Date) != 0)
                        j++;
                }
                PersonDate[] tempMassDates = new PersonDate[j];

                for (int c3 = 0; c3 < personDates.Length; c3++)
                {
                    if (howDaysToB(massPeople[c3].Date) < 14 && howDaysToB(massPeople[c3].Date) != 0)
                    {
                        tempMassDates[k] = massPeople[c3];
                        k++;
                    }
                }
                sortByDate(tempMassDates);

                for (int l = 0; l < k; l++)
                {
                    tempMassDates[l].Print();
                }

                if (j == 0)
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



            // Начало работы
            getPersonFromBD();
            PrintNextBirthday(massPeople);

            // цикл с выводом меню
            string commandMenu;
            string commandDopMenu;
            bool tempBool;
            bool tempBoolSec;
            do
            {
                tempBool = true;
                PrintMenuTop();
                commandMenu = Console.ReadLine();

                switch (commandMenu)
                {
                    case "1":
                        {
                            tempMassSort = massPeople;
                            Console.Clear();
                            PrintAllBithday(massPeople);
                            Console.WriteLine();
                            do
                            {
                                tempBoolSec = true;
                                PrintMenuDop();
                                commandDopMenu = Console.ReadLine();
                                switch (commandDopMenu)
                                {
                                    case "1":{
                                            Console.Clear();
                                            sortByDate(tempMassSort);
                                            for (int i = 0; i < tempMassSort.Length; i++)
                                            {
                                                tempMassSort[i].Print();
                                            }
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "2":{
                                            Console.Clear();
                                            PrintNextBirthday(tempMassSort);
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "3": {
                                            Console.Clear();
                                            printLaterDates(tempMassSort);
                                            Console.ReadLine();
                                        }
                                        break;
                                    case "0":
                                        tempBoolSec = false;
                                        Console.Clear();
                                        break;
                                    default:
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Вы ввели неверное значение");
                                        }
                                        break;
                                }

                            } while (tempBoolSec);
                            
                        }
                        break;
                    case "2":
                        {
                            Console.Clear();
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
                            tPerson = deletePersonDate();
                            tPerson.DeleteFromBD(conn);
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
                        tempBool = false;
                        break;
                    default:
                        {
                            Console.Clear();
                            Console.WriteLine("Вы ввели неверное значение");
                        }
                        break;
                }

            }
            while (tempBool);


            conn.Close();
        }
    }
}
