using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Lớp này chứa dữ liệu lưu tùy chọn của trò chơi.
/// </summary>
[System.Serializable]
public class OptionsSaveData
{
    public bool fullScreenMode = true;
    public Resolution? resolution = null;
    public bool vSync = true;

    public List<int> keyMapping = new List<int>();
    public List<int> buttonMapping = new List<int>();

    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float effectsVolume = 1.0f;

    public float gamepadVibration = 1.0f;
    public bool screenShake = true;
    public bool screenFlashes = true;

    public int language = (int)LanguageManager.Language.Last;
}

/// <summary>
/// Đây là lớp chứa dữ liệu lưu của trò chơi.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int playTime;                                        // thời gian chơi của người chơi
    public string sceneName;                                    // Tên cảnh của vị trí đã lưu
    public Vector2 playerPos;                                   // tọa độ của người chơi
    public bool hasLearnedClimbingWall;                         // Check player đã học được cách nhảy tường hay chưa
    public bool hasLearnedDoubleJump;                           // Check player đã học cách nhảy đôi hay chưa
    public List<Vector2> foundMaps = new List<Vector2>();       // Danh sách các bản đồ được người chơi khám phá cho đến nay
    public List<string> deadBosses = new List<string>();        // Danh sách trùm mà người chơi đã đánh bại cho đến nay
    public List<string> seenTutorials = new List<string>();     // Danh sách hướng dẫn người chơi đã xem cho đến nay

    public GameSaveData(int playTime,
                        string sceneName,
                        Vector2 playerPos,
                        bool hasLearnedClimbingWall,
                        bool hasLearnedDoubleJump,
                        List<Vector2> foundMaps,
                        List<string> deadBosses,
                        List<string> seenTutorials)
    {
        this.playTime = playTime;
        this.sceneName = sceneName;
        this.playerPos = playerPos;
        this.hasLearnedClimbingWall = hasLearnedClimbingWall;
        this.hasLearnedDoubleJump = hasLearnedDoubleJump;
        this.foundMaps = foundMaps;
        this.deadBosses = deadBosses;
        this.seenTutorials = seenTutorials;
    }
}

/// <summary>
/// lớp tĩnh xử lý hệ thống lưu để lưu trữ và tải dữ liệu của trò chơi.
/// </summary>
public static class SaveSystem
{
    // Khóa riêng được sử dụng để mã hóa
    private static readonly string privateKey = "EKDe$BMqvxvVf6z^ovKrhuf%JIJUg01CgCnadXYcOeGT%Iu5kS0xrj^09%^N";

    // New base path
    private static readonly string basePath = Path.Combine(Application.persistentDataPath.Replace("Hyeon Lee/Misty Traveler", "CongAnh/DungeonExplorer"));

    /// <summary>
    /// phương pháp tĩnh lưu dữ liệu tùy chọn vào một tệp
    /// </summary>
    /// <param name="optionsSaveData">Dữ liệu tùy chọn để lưu</param>
    public static void OptionSave(OptionsSaveData optionsSaveData)
    {
        // Chuyển đổi dữ liệu tùy chọn thành JSON, mã hóa và lưu vào tệp.
        string jsonString = JsonUtility.ToJson(optionsSaveData);
        string encryptString = Encrypt(jsonString);

        using (FileStream fs = new FileStream(OptionsGetPath(), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
            fs.Write(bytes, 0, bytes.Length);
        }
#if UNITY_EDITOR
        Debug.Log("Save Success: " + OptionsGetPath());
#endif
    }

    /// <summary>
    /// phương pháp tĩnh tải dữ liệu tùy chọn từ một tệp.
    /// </summary>
    /// <returns>Đã tải dữ liệu tùy chọn</returns>
    public static OptionsSaveData OptionLoad()
    {
        // Kiểm tra xem tệp có tồn tại hay không và trả về null nếu nó không tồn tại.
        if (!File.Exists(OptionsGetPath()))
        {
#if UNITY_EDITOR
            Debug.Log("Tệp đã lưu không tồn tại.");
#endif
            return null;
        }

        // Đọc dữ liệu được mã hóa từ một tập tin.
        string encryptData;
        using (FileStream fs = new FileStream(OptionsGetPath(), FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            encryptData = System.Text.Encoding.UTF8.GetString(bytes);
        }

        // Giải mã và trả về dữ liệu
        string decryptData = Decrypt(encryptData);
        OptionsSaveData saveData = JsonUtility.FromJson<OptionsSaveData>(decryptData);
        return saveData;
    }

    /// <summary>
    /// phương pháp tĩnh lưu dữ liệu trò chơi vào một tệp.
    /// </summary>
    /// <param name="saveData">Dữ liệu trò chơi cần lưu</param>
    /// <param name="dataNum">Lưu mã định danh tập tin</param>
    public static void GameSave(GameSaveData saveData, int dataNum)
    {
        // Chuyển đổi dữ liệu trò chơi sang JSON, mã hóa và lưu vào một tệp.
        string jsonString = JsonUtility.ToJson(saveData);
        string encryptString = Encrypt(jsonString);

        using (FileStream fs = new FileStream(GetPath(dataNum), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
            fs.Write(bytes, 0, bytes.Length);
        }

#if UNITY_EDITOR
        Debug.Log("Save Success: " + GetPath(dataNum));
#endif
    }

    /// <summary>
    /// phương pháp tĩnh lưu dữ liệu trò chơi vào một tệp với tên tệp tùy chỉnh.
    /// </summary>
    /// <param name="saveData">Dữ liệu trò chơi cần lưu</param>
    /// <param name="customFileName">Tên tệp tùy chỉnh</param>
    public static void GameSaveWithCustomName(GameSaveData saveData, string customFileName)
    {
        // Chuyển đổi dữ liệu trò chơi sang JSON, mã hóa và lưu vào một tệp.
        string jsonString = JsonUtility.ToJson(saveData);
        string encryptString = Encrypt(jsonString);

        using (FileStream fs = new FileStream(GetCustomPath(customFileName), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
            fs.Write(bytes, 0, bytes.Length);
        }

#if UNITY_EDITOR
        Debug.Log("Save Success: " + GetCustomPath(customFileName));
#endif
    }

    /// <summary>
    /// phương pháp tĩnh tải dữ liệu trò chơi từ một tệp.
    /// </summary>
    /// <param name="dataNum">Mã định danh của tệp đã lưu để tải</param>
    /// <returns>Đã tải dữ liệu trò chơi</returns>
    public static GameSaveData GameLoad(int dataNum)
    {
        // Kiểm tra xem tập tin có tồn tại không
        if (!File.Exists(GetPath(dataNum)))
        {
#if UNITY_EDITOR
            Debug.Log(dataNum + "Tệp lưu không tồn tại.");
#endif
            return null;
        }

        // Đọc dữ liệu được mã hóa từ một tập tin, giải mã nó và trả về nó.
        string encryptData;
        using (FileStream fs = new FileStream(GetPath(dataNum), FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            encryptData = System.Text.Encoding.UTF8.GetString(bytes);
        }

        string decryptData = Decrypt(encryptData);
#if UNITY_EDITOR
        Debug.Log(decryptData);
#endif
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(decryptData);
        return saveData;
    }

    /// <summary>
    /// phương pháp tĩnh để xóa một tệp lưu dữ liệu trò chơi cụ thể.
    /// </summary>
    /// <param name="dataNum">Mã định danh của tệp đã lưu sẽ bị xóa</param>
    public static void GameDelete(int dataNum)
    {
        if (!File.Exists(GetPath(dataNum)))
        {
#if UNITY_EDITOR
            Debug.Log("File lưu đó không tồn tại.");
#endif
            return;
        }
        File.Delete(GetPath(dataNum));
    }

    /// <summary>
    /// phương pháp tĩnh tạo và truy xuất đường dẫn tệp đến tệp lưu dữ liệu trò chơi.
    /// </summary>
    /// <param name="dataNum">Mã định danh để sử dụng khi tạo đường dẫn tệp</param>
    /// <returns>Đường dẫn tệp được tạo</returns>
    static string GetPath(int dataNum) => Path.Combine(basePath + @"/save0" + dataNum + ".save");

    /// <summary>
    /// phương pháp tĩnh tạo và truy xuất đường dẫn tệp đến tệp lưu dữ liệu trò chơi với tên tùy chỉnh.
    /// </summary>
    /// <param name="customFileName">Tên tệp tùy chỉnh</param>
    /// <returns>Đường dẫn tệp được tạo</returns>
    static string GetCustomPath(string customFileName) => Path.Combine(basePath + "/" + customFileName + ".save");

    /// <summary>
    /// phương thức tĩnh tạo và truy xuất đường dẫn tệp cho tệp dữ liệu tùy chọn.
    /// </summary>
    /// <returns>Đường dẫn tệp được tạo</returns>
    static string OptionsGetPath() => Path.Combine(basePath + @"/options.save");

    /// <summary>
    /// Phương thức tĩnh để mã hóa dữ liệu JSON
    /// </summary>
    /// <param name="data">Dữ liệu JSON bạn muốn mã hóa</param>
    /// <returns>dữ liệu được mã hóa</returns>
    static string Encrypt(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);
    }

    /// <summary>
    /// Phương thức tĩnh để giải mã dữ liệu JSON.
    /// </summary>
    /// <param name="data">Dữ liệu bạn muốn giải mã</param>
    /// <returns>dữ liệu được giải mã</returns>
    static string Decrypt(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    /// phương thức tĩnh tạo phiên bản RijndaelManaged
    /// </summary>
    /// <returns>Đã tạo phiên bản RijndaeManaged</returns>
    static RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }
}
