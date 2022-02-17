using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace AoOkami.SaveSystem
{
    public static class SaveSystem
    {
        private static readonly string _extension = ".save";

        public static void Save<T>(string path, T data)
        {
            string savePath = $"/{path + _extension}";
            FileStream file = new FileStream(Application.persistentDataPath + savePath, FileMode.OpenOrCreate);
            
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }
            catch (SerializationException e)
            {
                Debug.LogError("There was an issue serializing this data: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        
        public static T Load<T>(string path)
        {
            T data = default(T);
            string savePath = $"/{path + _extension}";

            if (File.Exists(Application.persistentDataPath + savePath))
            {
                FileStream file = new FileStream(Application.persistentDataPath + savePath, FileMode.Open);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = (T)formatter.Deserialize(file);
                }
                catch (SerializationException e)
                {
                    Debug.LogError("Error Deserializing Data: " + e.Message);
                }
                finally
                {
                    file.Close();
                }
            }

            return data;
        }

        public static void DeleteSaveFile(string path)
        {
            string filePath = $"{Application.persistentDataPath}/{path + _extension}";

            File.Delete(filePath);
        }
    }
}
