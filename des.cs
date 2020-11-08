using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DES
{
    public partial class Form1 : Form
    {
        private const int sizeBlock = 128; //подгоняем размер блока под unicode
        private const int sizeChar = 16; //размер одного символа 

        private const int shiftKey = 2; //сдвиг ключа 

        private const int quantityRounds = 16; //количество раундов

        String[] blocks; //блоки в двоичном формате

        String str = ""; //введенное сообщение
        String key = ""; //певый ключ
        String keyStorage = ""; //для хранения первого ключа 
        public Form1()
        {
            InitializeComponent();
        }

        /*Шифрование сообщения*/

        private void encrypt_Click(object sender, EventArgs e)
        {
            str = textBox1.Text.ToString();
            if (str == "")
            {
                MessageBox.Show("Введите сообщение, которое необходимо зашифровать!");
            }

            String result = "";

            /*Генерация ключа*/

            key = keyGen();
            result = encryption(key, str);
            keyStorage = keyStorage;

           

            /*Вывод результата*/

            textBox1.Text = result;
        }
        /*Шифрование с помощью ключа IP*/

        private string encryption(String key, String str)
        {
            String result = "";

            /*Разбить строку в символьном формате на блоки*/
            blocks = new string[(input.Length * sizeChar) / sizeBlock];

            int lengthBlock = input.Length / blocks.Length;

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = input.Substring(i * lengthBlock, lengthBlock);
                blocks[i] = stringToBinary(blocks[i]);
            }

            key = keyLenght(key, str.Length / (2 * blocks.Length));
            key = stringToBinary(key);

            for (int j = 0; j < quantityRounds; j++)
            {
                for (int i = 0; i < blocks.Length; i++)
                    blocks[i] = encryptionRound(blocks[i], key);

                key = nextRound(key);
            }

            key = prevoiusRound(key);
            keyStorage = stringToNormal(key);

            for (int i = 0; i < blocks.Length; i++)
            {
                result += blocks[i];
            }

            return stringToNormal(result);
        }

        /*Раунд шифрования*/

        private string encryptionRound(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }

        /*Дешифрование сообщения*/

        private void decrypt_Click(object sender, EventArgs e)
        {

            str = textBox1.Text.ToString();
            if (str == "")
            {
                MessageBox.Show("Введите сообщение, которое необходимо зашифровать!");
            }

            String result = "";

            /*Дешифрование ключом*/

            result = decryption(key, keyStorage, str);
            keyStorage = keyStorage;

            

            /*Вывод результата*/

            textBox1.Text = result;
        }


        /*Дешифрование при помощи ключа зашифрованного сообщения*/

        private string decryption(String key, String keyStorage, String str)
        {
            String result = "";

            key = stringToBinary(keyStorage);

            str = stringToBinary(str);
            /*Разбиваем строку двоичного формата на блоки*/
            blocks = new string[input.Length / sizeBlock];

            int lengthBlock = input.Length / blocks.Length;

            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = input.Substring(i * lengthBlock, lengthBlock);

            for (int j = 0; j < quantityRounds; j++)
            {
                for (int i = 0; i < blocks.Length; i++)
                    blocks[i] = decryptionRound(blocks[i], key);

                key = prevoiusRound(key);
            }

            key = nextRound(key);
            keyStorage = stringToNormal(key);

            result = "";
            for (int i = 0; i < blocks.Length; i++)
                result += blocks[i];

            return stringToNormal(result);
        }


        /*Генерация ключа*/

        private string keyGen()
        {
            Random random = new Random();
            return random.Next(10000, 100000).ToString();
        }
     
        /*Раунд десшифрования*/

        private string decryptionRound(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        /* Метод, доводящий строку до такого размера, чтобы она делилась на 128*/

        private string stringToSizeBlock(string input)
        {
            while (((input.Length * sizeChar) % sizeBlock) != 0)
                input += "#";

            return input;
        }

        /*Метод, переводящий строку в двоичный формат*/

        private string stringToBinary(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                string charBinary = Convert.ToString(input[i], 2);

                while (charBinary.Length < sizeChar)
                    charBinary = "0" + charBinary;

                output += charBinary;
            }

            return output;
        }

        /*Метод, переводящий строку с двоичными данными в символьный формат*/

        private string stringToNormal(string input)
        {
            string output = "";

            while (input.Length > 0)
            {
                string char_binary = input.Substring(0, sizeChar);
                input = input.Remove(0, sizeChar);

                int a = 0;
                int degree = char_binary.Length - 1;

                foreach (char c in char_binary)
                    a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

                output += ((char)a).ToString();
            }

            return output;
        }


        /*Метод, доводящий длину ключа до нужной длины*/

        private string keyLenght(string input, int lengthKey)
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
                while (input.Length < lengthKey)
                    input = "0" + input;

            return input;
        }

        /*XOR двух строк с двоичными данными*/

        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

        /*Функция f*/

        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }

        /*Вычисление ключа для следующего раунда шифрования*/

        private string nextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }

        /*Вычисление ключа для предыдущего раунда (для расшифрования)*/

        private string prevoiusRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }

            return key;
        }

     
    }
}
