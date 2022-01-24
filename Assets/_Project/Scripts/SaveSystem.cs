using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Crush2048
{
    public static class SaveSystem
    {
        private static readonly string _extension = ".save";

        public static void Save(string path, int highscore)
        {
            string savePath = $"/{path + _extension}";
            FileStream file = new FileStream(Application.persistentDataPath + savePath, FileMode.OpenOrCreate);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, highscore);
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

        public static int Load(string path)
        {
            int highscore = 0;
            string savePath = $"/{path + _extension}";

            if (File.Exists(Application.persistentDataPath + savePath))
            {
                FileStream file = new FileStream(Application.persistentDataPath + savePath, FileMode.Open);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    highscore = (int)formatter.Deserialize(file);
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

            return highscore;
        }
    }
}
