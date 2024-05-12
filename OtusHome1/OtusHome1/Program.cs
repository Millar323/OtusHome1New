// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

string connString = "Server=localhost;Port=5432;Database=my_db;Username=root;Password=root;SSLMode=Prefer";

Console.WriteLine("Вызвать скрипт создания и заполнения таблиц - 1;");
Console.WriteLine("Показать данные всех таблиц - 2;");
Console.WriteLine("Добавить запись в одну из таблиц на выбор - 3;");
var x = Console.ReadLine();
if (x == "1") 
{
    DBScripts();
}
else if (x == "2")
{
    using (var conn = new NpgsqlConnection(connString)) 
    {
        Console.Out.WriteLine("Opening connection");
        conn.Open();

        using (var command = new NpgsqlCommand("select * from Customer", conn))
        {
            using (var rows = command.ExecuteReader()) 
            {
                Console.WriteLine("|{0}|\t|{1}|\t|{2}|", rows.GetName(0), rows.GetName(1), rows.GetName(2));
                foreach (var a in rows) 
                {
                    var id = rows.GetValue(0);
                    var name = rows.GetValue(1);
                    var age = rows.GetValue(2);

                    Console.WriteLine("|{0}|\t|{1}|\t|{2}|", id, name, age);
                }
            }
        }

        using (var command = new NpgsqlCommand("select * from Products", conn))
        {

            using (var rows = command.ExecuteReader())
            {
                Console.WriteLine("|{0}|\t|{1}|", rows.GetName(0), rows.GetName(1));
                foreach (var a in rows)
                {
                    var id = rows.GetValue(0);
                    var type = rows.GetValue(1);

                    Console.WriteLine("|{0}|\t|{1}|", id, type);
                }
            }
        }

        using (var command = new NpgsqlCommand("select * from CustomersProducts", conn))
        {
            using (var rows = command.ExecuteReader())
            {
                Console.WriteLine("|{0}|\t|{1}|\t|{2}|", rows.GetName(0), rows.GetName(1), rows.GetName(2));
                foreach (var a in rows)
                {
                    var id = rows.GetValue(0);
                    var customerId = rows.GetValue(1);
                    var productId = rows.GetValue(2);

                    Console.WriteLine("|{0}|\t|{1}|\t|{2}|", id, customerId, productId);
                }
            }
        }
    }
}
else if (x == "3")
{
    Console.WriteLine("Выберите таблицу:");

    Console.WriteLine("Customer - 1;");
    Console.WriteLine("Products - 2;");
    Console.WriteLine("CustomersProducts - 3;");
    var y = Console.ReadLine();

    using (var conn = new NpgsqlConnection(connString))
    {
        conn.Open();
        if (y == "1")
        {
            Console.WriteLine("Введите имя:");
            var name = Console.ReadLine();

            Console.WriteLine("Введите возраст:");
            var age = Console.ReadLine();

            using (var command = new NpgsqlCommand("insert into Customer (name, age) VALUES (@v1, @v2)", conn))
            {
                command.Parameters.AddWithValue("v1", name);
                command.Parameters.AddWithValue("v2", age);
                command.ExecuteNonQuery();
            }
        }
        else if (y == "2")
        {
            Console.WriteLine("Введите название продукта:");
            var type = Console.ReadLine();

            using (var command = new NpgsqlCommand("insert into Products (type) VALUES (@v1)", conn))
            {
                command.Parameters.AddWithValue("v1", type);
                command.ExecuteNonQuery();
            }
        }
        else if (y == "3")
        {
            Console.WriteLine("Введите идентификатор клиента:");
            var customerId = Console.ReadLine();

            Console.WriteLine("Введите идентификатор продукта:");
            var productId = Console.ReadLine();

            using (var command = new NpgsqlCommand("insert into CustomersProducts (CustomerId, ProductId) VALUES (@v1, @v2)", conn))
            {
                command.Parameters.AddWithValue("v1", customerId);
                command.Parameters.AddWithValue("v2", productId);
                command.ExecuteNonQuery();
            }
        }
    }
}

Console.WriteLine("Press RETURN to exit");
Console.ReadLine();
void DBScripts() 
{
    using (var conn = new NpgsqlConnection(connString))
    {
        Console.Out.WriteLine("Opening connection");
        conn.Open();

        using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS customer cascade; DROP TABLE IF EXISTS products cascade; DROP TABLE IF EXISTS CustomersProducts cascade;", conn))
        {
            command.ExecuteNonQuery();
            Console.Out.WriteLine("Finished dropping table (if existed)");

        }

        using (var command = new NpgsqlCommand("create table Customer (Id serial primary key, Name varchar(100) not null, Age integer not null); create table Products (Id serial primary key, Type varchar(50) not null); create table CustomersProducts ( Id serial primary key, CustomerId integer, ProductId integer, foreign key (CustomerId) references Customer (Id), foreign key (ProductId) references Products (Id));", conn))
        {
            command.ExecuteNonQuery();
            Console.Out.WriteLine("Finished creating table");
        }

        var customers = new Dictionary<string, int>()
        {
            { "Иванов Иван Иванович", 33 },
            { "Петров Петр Петрович", 45 },
            { "Артемов Артем Артемович", 26 },
            { "Сергеев Сергей Сергеевич", 56 },
            { "Александров Александр Александрович", 60 }
        };
        foreach (var x in customers)
        {
            using (var command = new NpgsqlCommand("insert into Customer (name, age) VALUES (@v1, @v2)", conn))
            {
                command.Parameters.AddWithValue("v1", x.Key);
                command.Parameters.AddWithValue("v2", x.Value);
                command.ExecuteNonQuery();
            }
        }

        var products = new List<string>()
        {
            "Кредит",
            "Рассрочка",
            "Кредитная карта",
            "Дебетовая карта",
            "Сберегательный счет"
        };
        foreach (var x in products)
        {
            using (var command = new NpgsqlCommand("insert into Products (type) VALUES (@v1)", conn))
            {
                command.Parameters.AddWithValue("v1", x);
                command.ExecuteNonQuery();
            }
        }

        var customersProducts = new Dictionary<int, int>()
        {
            { 1, 2 },
            { 2, 3 },
            { 3, 5 },
            { 4, 2 },
            { 5, 4 }
        };
        foreach (var x in customersProducts)
        {
            using (var command = new NpgsqlCommand("insert into CustomersProducts (CustomerId, ProductId) VALUES (@v1, @v2)", conn))
            {
                command.Parameters.AddWithValue("v1", x.Key);
                command.Parameters.AddWithValue("v2", x.Value);
                command.ExecuteNonQuery();
            }
        }
    }

}

