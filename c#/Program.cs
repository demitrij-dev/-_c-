using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.IO;

namespace Program
{
    public class SignXML
    {
        public static void Main()
        {
            Console.WriteLine("Введите путь до XML файла:");
            string filePath = Console.ReadLine();

            if (!ValidateFilePath(filePath))
            {
                Console.WriteLine("Путь к файлу некорректен или файл недоступен.");
                return;
            }

            try
            {
                using RSA rsaKey = RSA.Create(2048);
                
                XmlDocument xmlDoc = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDoc.Load(filePath);

                SignXml(xmlDoc, rsaKey);

                Console.WriteLine("XML файл подписан.");

                string signedFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_signed" + 
                    Path.GetExtension(filePath)
                );

                xmlDoc.Save(signedFilePath);
                Console.WriteLine($"Подписанный XML файл сохранен: {signedFilePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.Message);
            }
        }

        public static void SignXml(XmlDocument xmlDoc, RSA rsaKey)
        {
            if (xmlDoc == null)
                throw new ArgumentException(nameof(xmlDoc));
            if (rsaKey == null)
                throw new ArgumentException(nameof(rsaKey));

            SignedXml signedXml = new SignedXml(xmlDoc)
            {
                SigningKey = rsaKey
            };

            Reference reference = new Reference
            {
                Uri = ""
            };

            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            
            signedXml.AddReference(reference);
            
            signedXml.ComputeSignature();
            
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
        }

        public static bool ValidateFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            if (Path.GetExtension(filePath).ToLower() != ".xml")
            {
                return false;
            }

            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {

                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}