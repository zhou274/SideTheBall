﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class Board : MonoBehaviour
{
    private Dictionary<Vector3, Tile> tiles = new Dictionary<Vector3, Tile>();
    public int size = 4;
    public RectTransform rt;
    public Text currentMoveText, targetMoveText, bestMoveText;
    public TextMeshProUGUI worldNameText;
    public TextMeshProUGUI levelNameText;
    public Transform hintRegion, starFlyingRegion, ballRegion;
    public Transform[] headerStarTransforms;
    public Button undoButton, redoButton;

    public static Board instance;

    private List<AMove> moves = new List<AMove>();
    private Tile startTile, goalTile;
    private GameObject ball;
    private List<Tile> pathTiles;
    private int moveBallIndex, starCollected, numStarLoaded, moveCursor = -1;
    private Level level;
    public string clickid;
    private StarkAdManager starkAdManager;
    private int _currentMove, _targetMove, _bestMove;
    private int currentMove
    {
        get { return _currentMove; }
        set { _currentMove = value; currentMoveText.text = _currentMove.ToString(); }
    }

    private int targetMove
    {
        get { return _targetMove; }
        set { _targetMove = value; targetMoveText.text = _targetMove == 0 ? "-" : _targetMove.ToString(); }
    }

    private int bestMove
    {
        get { return _bestMove; }
        set { _bestMove = value; bestMoveText.text = _bestMove == -1 ? "-" : _bestMove.ToString(); }
    }

    private void Start()
    {
        currentMove = 0;
        bestMove = Prefs.bestMove;
        targetMove = level.targetMove;

        worldNameText.text = "关卡包 " + (Prefs.currentWorld + 1);
        levelNameText.text = "关卡 " + (Prefs.currentLevel + 1);
        UpdateUndoRedoButton();
    }

    private void Awake()
    {
        instance = this;
    }

    public void LoadLevel(Level level)
    {
        ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
        this.level = level;
        size = level.size;

        int index = 0;
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                if (index < level.maps.Length)
                {
                    TileP p = level.maps[index];
                    if (p.type != Tile.Type.None)
                    {
                        Tile tile = Instantiate(MonoUtils.instance.tilePrefab);
                        tile.transform.SetParent(transform);
                        tile.transform.localScale = Vector3.one;
                        tile.p = p;
                        tile.position = new Vector3(col, row);
                        tile.width = rt.rect.width / size;
                        tile.onTileMoveComplete += OnMoveTileComplete;

                        tile.transform.localPosition = tile.localPosition;
                        tiles.Add(tile.position, tile);

                        if (p.type == Tile.Type.Start)
                        {
                            startTile = tile;
                        }
                        else if (p.type == Tile.Type.Goal)
                        {
                            goalTile = tile;
                        }

                        if (numStarLoaded == 3 && p.hasStar)
                        {
                            p.hasStar = false;
                        }

                        if (p.hasStar)
                        {
                            GameObject star = Instantiate(MonoUtils.instance.starPrefab);
                            star.transform.SetParent(tile.transform);
                            star.transform.localScale = Vector3.one;
                            star.transform.localPosition = Vector3.zero;
                            tile.star = star;
                            SetStarPosition(tile, star);
                            numStarLoaded++;
                        }
                    }
                }
                index++;
            }
        }

        List<Vector3[]> hintPaths = new List<Vector3[]>();
        for(int i = 0; i < level.hintPath.Length; i++)
        {
            if (i == 0)
            {
                hintPaths.Add(new Vector3[1] { level.hintPath[i + 1] - level.hintPath[i] });
            }
            else if (i == level.hintPath.Length - 1)
            {
                hintPaths.Add(new Vector3[1] { level.hintPath[i - 1] - level.hintPath[i] });
            }
            else
            {
                hintPaths.Add(new Vector3[2] { level.hintPath[i - 1] - level.hintPath[i], level.hintPath[i + 1] - level.hintPath[i] });
            }
        }

        index = 0;
        foreach (var hintPos in level.hintPath)
        {
            TileHint tileHint = Instantiate(MonoUtils.instance.tileHintPrefab);
            tileHint.transform.SetParent(hintRegion);
            tileHint.transform.localScale = Vector3.one;
            tileHint.width = rt.rect.width / size;

            tileHint.position = hintPos;
            tileHint.transform.localPosition = tileHint.localPosition;

            tileHint.paths = hintPaths[index];
            tileHint.UpdateUI();
            index++;
        }

        HideHint();

        ball = Instantiate(MonoUtils.instance.ballPrefab);
        ball.transform.SetParent(ballRegion);
        ball.transform.localScale = Vector3.one;
        ball.transform.localPosition = startTile.localPosition;
    }

    private void SetStarPosition(Tile tile, GameObject star)
    {
        if (tile.p.paths.Length < 2)
            star.transform.localPosition = Vector3.zero;
        else
        {
            var path1 = GetPathVector(tile.p.paths[0]);
            var path2 = GetPathVector(tile.p.paths[1]);
            
            if (path1 + path2 == Vector3.zero)
            {
                star.transform.localPosition = Vector3.zero;
            }
            else
            {
                star.transform.localPosition = (path1 + path2) * 0.13f * tile.width;
            }
        }
    }

    public bool CanMove(Tile tile, Vector3 direct)
    {
        Vector3 newPosition = tile.position + direct;
        if (newPosition.x >= size || newPosition.x < 0 || newPosition.y >= size || newPosition.y < 0) return false;
        return !tiles.ContainsKey(newPosition);
    }

    public void UndoMove()
    {
        if (MainController.instance.isComplete) return;

        var trackMove = moves[moveCursor];
        var tile = trackMove.tile;
        iTween.MoveTo(tile.gameObject, iTween.Hash("position", tile.GetLocalPosition(trackMove.fromPosition), "isLocal", true, "time", 0.08f));

        ChangeTilePosition(tile, trackMove.fromPosition);
        currentMove--;
        moveCursor--;

        UpdateUndoRedoButton();
        Sound.instance.Play(Sound.Others.Slide);
    }

    public void RedoMove()
    {
        if (MainController.instance.isComplete) return;

        moveCursor++;

        var trackMove = moves[moveCursor];
        var tile = trackMove.tile;
        iTween.MoveTo(tile.gameObject, iTween.Hash("position", tile.GetLocalPosition(trackMove.toPosition), "isLocal", true, "time", 0.08f));

        ChangeTilePosition(tile, trackMove.toPosition);
        currentMove++;

        UpdateUndoRedoButton();
        Sound.instance.Play(Sound.Others.Slide);
    }

    private void UpdateUndoRedoButton()
    {
        bool undoActive = moveCursor > -1;
        bool redoActive = moveCursor < moves.Count - 1;

        undoButton.interactable = undoActive;
        redoButton.interactable = redoActive;
    }

    private void ChangeTilePosition(Tile tile, Vector3 newPosition)
    {
        tiles.Remove(tile.position);
        tile.position = newPosition;
        tiles.Add(newPosition, tile);
    }

    public void OnMoveTileComplete(Tile tile, Vector3 newPosition)
    {
        for(int i = moves.Count - 1; i > moveCursor; i--)
        {
            moves.RemoveAt(i);
        }

        var aMove = new AMove
        {
            tile = tile,
            fromPosition = tile.position,
            toPosition = newPosition
        };
        moves.Add(aMove);
        moveCursor = moves.Count - 1;

        UpdateUndoRedoButton();

        ChangeTilePosition(tile, newPosition);

        currentMove++;
        
        pathTiles = new List<Tile>();
        bool complete = CheckComplete();

        if (complete)
        {
            if (currentMove < bestMove || bestMove == -1)
            {
                bestMove = currentMove;
                Prefs.bestMove = currentMove;
            }
            
            MoveBall();
            MainController.instance.OnComplete();
        }
    }

    private bool CheckComplete()
    {
        Tile cursor = startTile;
        var outPath = cursor.p.paths[0];
        pathTiles.Add(cursor);

        while (true)
        {
            
            var nextTile = GetTile(cursor.position + GetPathVector(outPath));
            if (nextTile == null || nextTile.p.paths.Length == 0) return false;

            bool match = false;
            Tile.Path newOutpath = Tile.Path.Down;

            foreach (var path in nextTile.p.paths)
            {
                if (GetPathVector(path) + GetPathVector(outPath) == Vector3.zero)
                {
                    match = true;
                }
                else
                {
                    newOutpath = path;
                }
            }

            outPath = newOutpath;
            cursor = nextTile;
            pathTiles.Add(cursor);

            if (match == false) return false;
            else if (nextTile == goalTile) return true;
            
        }
    }

    private Queue<GameObject> flyingStars = new Queue<GameObject>();
    private void CollectStar(GameObject star)
    {
        Vector3 begin = star.transform.position;
        Vector3 end = headerStarTransforms[starCollected].position;
        Vector3 middle = CUtils.GetMiddlePoint(begin, end, -0.5f);

        Vector3[] waypoints = { begin, middle, end };

        star.transform.SetParent(starFlyingRegion);
        flyingStars.Enqueue(star);

        star.GetComponent<PlaybackImageSequence>().fps = 50f;
        iTween.MoveTo(star, iTween.Hash("path", waypoints, "time", 0.7f, "oncomplete", "OnStarMoveComplete", "oncompletetarget", gameObject));

        starCollected++;
    }

    private void OnStarMoveComplete()
    {
        foreach(var tr in headerStarTransforms)
        {
            if (!tr.gameObject.activeSelf)
            {
                tr.gameObject.SetActive(true);
                tr.localScale = Vector3.zero;
                iTween.ScaleTo(tr.gameObject, iTween.Hash("scale", Vector3.one, "isLocal", true, "time", 0.2f));
                break;
            }
        }

        Destroy(flyingStars.Dequeue());
    }

    private Tile.Path outPath;
    private void MoveBall()
    {
        if (moveBallIndex >= pathTiles.Count)
        {
            int numstar = 0;
            if (level.mode == Level.LevelMode.Star)
            {
                numstar = starCollected;
            }
            else
            {
                float quanlity = targetMove / (float)currentMove;
                if (quanlity >= 1) numstar = 3;
                else if (quanlity >= 0.75f) numstar = 2;
                else numstar = 1;
            }

            Prefs.SetNumStar(Prefs.currentWorld, Prefs.currentLevel, numstar);

            MainController.instance.OnBallToGoal();
            Sound.instance.Play(Sound.Others.BallEnd);

            return;
        }

        Tile tile = pathTiles[moveBallIndex];

        if (tile.p.hasStar)
        {
            Timer.Schedule(this, 0.1f, () =>
            {
                CollectStar(tile.star);
                Sound.instance.Play(Sound.Others.GetStar);
            });
        }

        Vector3? destination = null;
        Vector3[] waypoints = null;

        if (tile.p.paths.Length == 1)
        {
            if (tile == startTile)
            {
                outPath = startTile.p.paths[0];
                destination = tile.localPosition + tile.width / 2 * GetPathVector(outPath);
            }
            else
            {
                destination = goalTile.localPosition;
            }
        }
        else
        {
            var path1 = tile.p.paths[0];
            var path2 = tile.p.paths[1];

            Tile.Path inPath;
            if (GetPathVector(path1) + GetPathVector(outPath) == Vector3.zero)
            {
                inPath = path1;
                outPath = path2;
            }
            else
            {
                inPath = path2;
                outPath = path1;
            }

            var inPathVector = GetPathVector(inPath);
            var outPathVector = GetPathVector(outPath);

            if (inPathVector + outPathVector == Vector3.zero)
            {
                destination = tile.localPosition + tile.width / 2 * GetPathVector(outPath);
            }
            else
            {
                Vector3 middleVector = inPathVector + outPathVector;
                Vector3 point1 = tile.localPosition + tile.width / 3 * GetPathVector(inPath);
                Vector3 point2 = tile.localPosition + middleVector * tile.width * 0.14f;
                Vector3 point3 = tile.localPosition + tile.width / 3 * GetPathVector(outPath);
                Vector3 point4 = tile.localPosition + tile.width / 2 * GetPathVector(outPath);
                waypoints = new Vector3[4] {
                    transform.TransformPoint(point1),
                    transform.TransformPoint(point2),
                    transform.TransformPoint(point3),
                    transform.TransformPoint(point4) };
            }
        }

        if (destination != null)
        {
            iTween.MoveTo(ball, iTween.Hash("position", (Vector3)destination, "isLocal", true, "easeType", iTween.EaseType.linear, "speed", 500, "oncomplete", "MoveBall", "oncompletetarget", gameObject));
        }
        else
        {
            iTween.MoveTo(ball, iTween.Hash("path", waypoints, "isLocal", false, "easeType", iTween.EaseType.linear, "speed", 5f, "oncomplete", "MoveBall", "oncompletetarget", gameObject));
        }

        moveBallIndex++;
    }

    private Vector3 GetPathVector(Tile.Path path)
    {
        return  path == Tile.Path.Up ? Vector3.up :
                path == Tile.Path.Down ? Vector3.down :
                path == Tile.Path.Left ? Vector3.left : Vector3.right;
    }

    private Tile GetTile(Vector3 position)
    {
        return !tiles.ContainsKey(position) ? null : tiles[position];
    }

    [HideInInspector]
    public bool hintBeginShowing, hintShowing;
    private bool usedHint;

    public void ShowHint()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    if (hintBeginShowing || MainController.instance.isComplete) return;
                    Sound.instance.PlayButton();

                    if (hintShowing)
                    {
                        HideHint();
                        return;
                    }
                    usedHint = true;
                    StartCoroutine(IESHowHint());



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
        //if (usedHint || CurrencyController.DebitBalance(10))
        //{
        //    usedHint = true;
        //    StartCoroutine(IESHowHint());
        //}
        //else
        //{
        //    DialogController.instance.ShowDialog(DialogType.Shop);
        //}
    }

    public void HideHint()
    {
        foreach (Transform child in hintRegion)
        {
            child.gameObject.SetActive(false);
        }
        hintShowing = false;
    }

    private IEnumerator IESHowHint()
    {
        hintBeginShowing = true;
        foreach (Transform child in hintRegion)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        hintBeginShowing = false;
        hintShowing = true;
    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
