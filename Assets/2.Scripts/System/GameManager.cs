using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lớp Singleton quản lý các thành phần liên quan đến trò chơi.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // Thể hiện lớp Singleton

    // Sự kiện gọi khi người chơi chết
    public delegate void PlayerDieEventHandler();
    public event PlayerDieEventHandler PlayerDieEvent = null;

    /// <summary>
    /// Enum liệt kê trạng thái của trò chơi.
    /// </summary>
    public enum GameState
    {
        Title, // Tiêu đề
        Play,  // Chơi
        MenuOpen // Mở menu
    }
    public GameState currentGameState = GameState.Play;     // Trạng thái trò chơi hiện tại

    [HideInInspector] public bool firstStart = true;        // Kiểm tra xem liệu dữ liệu trò chơi đã được tạo và khởi đầu lần đầu chưa
    [HideInInspector] public Vector2 playerStartPos;        // Vị trí bắt đầu của người chơi
    [HideInInspector] public float playerStartlocalScaleX;  // Hướng nhìn của người chơi sau khi trò chơi bắt đầu

    [HideInInspector] public int playerCurrentHealth;       // Sức khỏe hiện tại của người chơi
    [HideInInspector] public int playerCurrentDrivingForce; // Lực đẩy hiện tại của người chơi

    [HideInInspector] public string resurrectionScene;      // Cảnh mà người chơi sẽ hồi sinh sau khi chết
    [HideInInspector] public Vector2 playerResurrectionPos; // Vị trí mà người chơi sẽ hồi sinh sau khi chết

    int gameDataNum = 1; // Số liệu trò chơi hiện tại đang chơi

    int _prevPlayTime;  // Thời gian chơi trước đó
    float _startTime;   // Thời gian bắt đầu chơi (được sử dụng để tính thời gian chơi)

    bool _isStarted = true; // Kiểm tra xem trò chơi đã bắt đầu chưa

    void Awake()
    {
        // Thiết lập lớp Singleton
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Cursor.visible = false; // Ẩn con trỏ chuột

        EnemyDataManager.Init();
        SceneManager.sceneLoaded += OnSceneLoaded;

        OptionsData.Init(); // Khởi tạo dữ liệu tùy chọn

#if UNITY_EDITOR
        // Nếu đang chạy trò chơi trong trình chỉnh sửa Unity, tải dữ liệu lưu trò chơi số 1
        if (currentGameState == GameState.Play)
        {
            GameLoad(1, false);
        }
#endif
    }

    /// <summary>
    /// Kiểm tra xem trò chơi đã bắt đầu chưa.
    /// </summary>
    /// <returns>Trả về true nếu trò chơi đã bắt đầu, ngược lại trả về false.</returns>
    public bool IsStarted()
    {
        if (_isStarted)
        {
            // Nếu trò chơi đã bắt đầu, thiết lập thời gian bắt đầu và đặt _isStarted thành false, sau đó trả về true
            _startTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
            _isStarted = false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Thiết lập trạng thái trò chơi hiện tại.
    /// Khi mở menu, thời gian chơi sẽ dừng lại; khi bắt đầu chơi, thời gian dừng sẽ bị hủy.
    /// </summary>
    /// <param name="newGameState">Trạng thái mới cần thiết lập</param>
    public void SetGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.Play:
                ScreenEffect.instance.TimeStopCancle();
                break;
            case GameState.MenuOpen:
                ScreenEffect.instance.TimeStopStart();
                break;
            case GameState.Title:
                break;
        }

        currentGameState = newGameState;
    }

    /// <summary>
    /// Chuyển đổi thời gian chơi (kiểu int) thành chuỗi văn bản để hiển thị.
    /// </summary>
    /// <param name="time">Thời gian chơi (kiểu int)</param>
    /// <returns>Chuỗi văn bản biểu diễn thời gian chơi</returns>
    public string IntToTimeText(int time)
    {
        int sec = (time % 60);
        int min = ((time / 60) % 60);
        int hour = (time / 3600);

        string secToStr = sec < 10 ? "0" + sec.ToString() : sec.ToString();
        string minToStr = min < 10 ? "0" + min.ToString() : min.ToString();
        string hourToStr = hour.ToString();

        return string.Format("{0}:{1}:{2}", hourToStr, minToStr, secToStr);
    }

    /// <summary>
    /// Xử lý sự kiện khi người chơi chết.
    /// </summary>
    public void HandlePlayerDeath()
    {
        playerStartPos = playerResurrectionPos;
        DeadEnemyManager.ClearDeadEnemies();
        PlayerDieEvent?.Invoke();
    }

    /// <summary>
    /// Lưu trạng thái hiện tại của trò chơi.
    /// </summary>
    public void GameSave()
    {
        string sceneName = resurrectionScene; // Đặt cảnh (điểm kiểm tra) nơi nhân vật người chơi được hồi sinh làm vị trí bắt đầu trò chơi
        List<string> seenTutorials = TutorialManager.GetSeenTutorialsToString();
        List<string> deadBosses = DeadEnemyManager.GetDeadBosses();
        List<Vector2> foundMaps = MapManager.GetDiscoveredMaps();
        int playTime = _prevPlayTime + Mathf.CeilToInt((float)DateTime.Now.TimeOfDay.TotalSeconds - _startTime);    // Tính toán thời gian chơi game

        var saveData = new GameSaveData
        (
            playTime,
            sceneName,
            playerResurrectionPos,
            PlayerLearnedSkills.hasLearnedClimbingWall,
            PlayerLearnedSkills.hasLearnedDoubleJump,
            foundMaps,
            deadBosses,
            seenTutorials
        );
        SaveSystem.GameSave(saveData, gameDataNum);
    }

    /// <summary>
    /// tải dữ liệu chơi trò chơi đã lưu.
    /// </summary>
    /// <param name="gameDataNum">Số dữ liệu trò chơi mà b muốn tải</param>
    /// <param name="sceneLoaded">Có tải cảnh hay không</param>
    public void GameLoad(int gameDataNum, bool sceneLoaded = true)
    {
        var saveData = SaveSystem.GameLoad(gameDataNum);
        this.gameDataNum = gameDataNum;

        if(saveData == null)
        {
            // Nếu dữ liệu lưu bạn muốn tải không tồn tại, điều đó được xác định rằng trò chơi đã được bắt đầu lần đầu tiên và bản đồ đầu tiên được tải và trả lại
            SceneTransition.instance.LoadScene("OldMachineRoom_A");
            return;
        }
        _prevPlayTime = saveData.playTime;  // Đặt thời gian chơi lưu dữ liệu về thời gian chơi trò chơi trước đó

        resurrectionScene = saveData.sceneName;     // Đặt cảnh người chơi hồi sinh khi chết thành cảnh bạn muốn tải từ bản lưu
        playerResurrectionPos = saveData.playerPos; // Đặt tọa độ hồi sinh người chơi

        // Nhận bản đồ được khám phá
        MapManager.AddDiscoveredMaps(saveData.foundMaps);
        // Nhập các kỹ năng đã học
        PlayerLearnedSkills.hasLearnedClimbingWall = saveData.hasLearnedClimbingWall;
        PlayerLearnedSkills.hasLearnedDoubleJump = saveData.hasLearnedDoubleJump;

        // Thêm danh sách trùm chết
        for (int i = 0; i < saveData.deadBosses.Count; i++)
        {
            DeadEnemyManager.AddDeadBoss(saveData.deadBosses[i]);
        }

        // Thêm danh sách các hướng dẫn đã xem
        for (int i = 0; i < saveData.seenTutorials.Count; i++)
        {
            TutorialManager.AddSeenTutorial(saveData.seenTutorials[i]);
        }

        firstStart = false; // xác định rằng đây không phải là lần đầu tiên trò chơi được bắt đầu sau khi tạo dữ liệu trò chơi

        if (sceneLoaded)
        {
            // tải cảnh khi sceneLoaded được đặt thành True
            playerStartPos = saveData.playerPos;
            SceneTransition.instance.LoadScene(saveData.sceneName);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nếu trạng thái trò chơi hiện tại là màn hình tiêu đề, dữ liệu sẽ được khởi tạo.
        if (currentGameState == GameState.Title)
        {
            gameDataNum = 1;

            firstStart = true;

            playerStartPos = Vector2.zero;
            playerStartlocalScaleX = 0;

            playerCurrentHealth = 0;
            playerCurrentDrivingForce = 0;

            resurrectionScene = string.Empty;
            playerResurrectionPos = Vector2.zero;

            _prevPlayTime = 0;
            _startTime = 0;

            _isStarted = true;
        }
        // Đặt lại sự kiện người chơi chết khi cảnh thay đổi.
        PlayerDieEvent = null;
    }

    void OnApplicationQuit()
    {
        // Tự động lưu trò chơi khi cửa sổ trò chơi đóng lại
        if (instance.currentGameState != GameState.Title)
        {
            instance.GameSave();
        }
        OptionsData.OptionsSave();
    }
}
