﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    const float CREATE_TIME = 1.0f; // ボート生成時間
    float passTime; // 生成時に使う経過時間
    int DownBoats;  // 沈めたボート数

	//援軍の船用パラメータ
	int MaxCompanyBoats = 5;
	int CurrentCompanyBoats = 0;
	const float CompanyBoatsCreateTime = 10.0f; // ボート生成時間
	float CompanyBoatsPassTime; // 生成時に使う経過時間

    // ボート生成時の最大、最小座標
    public float createMinX, createMaxX;
    public float createMinY, createMaxY;
    public GameObject japBoat;      // ボートObj
    public GameObject itemHeart;    // ハートアイテムObj
    public GameObject explosion;    // 爆発エフェクトObj

	private List<GameObject> japBoats = new List<GameObject>();	// ボートobjを保存しておくリスト
	private int maxJapBoats = 10;	// 漁船の最大数

    public Text ScoreLabel;         // スコア
	public Text GameOverLabel;      // ゲームオーバーテキスト
    public Button RetryButton;      // リトライボタン
    public HPPanel hpPanelScript;   // HP表示スクリプト

    DolphinController dolphinCtrl;

    float itemDropRatio;            // アイテムドロップ率
    bool IsGameOver;
    bool HasPushRetry;

    // Use this for initialization
    void Start () {
        passTime = Time.time;       // 経過時間に現在の時間を設定
        japBoat.SetActive(false);   // コピー元Objをdeactive

		// ゲームオーバー用にイルカの生存フラグをみる
        GameObject dolpObj = GameObject.FindWithTag("Player");
        dolphinCtrl = dolpObj.GetComponent<DolphinController>();

        itemDropRatio = 1 / 5.0f;   // アイテムドロップ率設定
        IsGameOver = false;
        HasPushRetry = false;

    }

    // Update is called once per frame
    void Update () {

        // ボート生成
        CreateBoat();
		//CreateCompanyBoat();

		// 沈められたボートListの削除
		DeleteJapBoats();

		// スコア表示
        ScoreLabel.text = "しずめた数 : " + DownBoats;

        // HP表示
        hpPanelScript.UpdateHPPanel(dolphinCtrl.GetHP());

        if (!dolphinCtrl.GetIsAlive())
        {
            if (!IsGameOver)
            {
                GameOver();
            }
        }
	}

    // ボート生成
    void CreateBoat()
    {
		// 漁船数が最大値ならそれ以上作成しない
		if (japBoats.Count >= maxJapBoats) return;
		
        // 生成時間を過ぎたらつくる
        float nowTime = Time.time;
        if ((Time.time - passTime) < CREATE_TIME) return;
        passTime = nowTime;

        // 座標はランダムで決定
        float createX = Random.Range(createMinX, createMaxX);
        float createY = Random.Range(createMinY, createMaxY);

		GameObject Obj = (GameObject)Instantiate(
            japBoat,
            new Vector3(createX, createY, 0.0f),
            Quaternion.identity);
		Obj.SetActive(true);
		japBoats.Add (Obj);

    }

	// 援軍のボート生成
	void CreateCompanyBoat()
	{
		// 生成できる上限なら抜ける
		if (CurrentCompanyBoats >= MaxCompanyBoats) return;

		// 生成時間を過ぎたらつくる
		if ((Time.time - CompanyBoatsPassTime) < CompanyBoatsCreateTime) return;
		CompanyBoatsPassTime = Time.time;

		// 座標はランダムで決定
		//float createX = Random.Range(createMinX, createMaxX);
		//float createY = Random.Range(createMinY, createMaxY);
		/*
		GameObject Obj = (GameObject)Instantiate(
			CompanyBoat,
			new Vector3(createX, createY, 0.0f),
			Quaternion.identity);
		Obj.SetActive(true);
		*/
	}

    // ゲームオーバー処理
    void GameOver()
    {
        if (HasPushRetry) return;

        Time.timeScale = 0.0f;
        IsGameOver = true;
        GameOverLabel.gameObject.SetActive(true);
		GameOverLabel.text = "m9(^Д^)";
        RetryButton.gameObject.SetActive(true);
    }

    // ボートが沈んだら
    // 現状、沈めたボート数を加算する
    public void DownBoat()
    {
        DownBoats++;
        Debug.Log("downs : " + DownBoats);
    }

    // リトライボタン押下時
    public void PushRetry()
    {
        Time.timeScale = 1.0f;
        IsGameOver = false;
        HasPushRetry = true;
        //Application.LoadLevel("DolphinJapan");	// 非推奨らしい
		SceneManager.LoadScene("DolphinJapan");		// 今後はこっちで
    }

    // アイテムドロップ
    public void DropItem(float dropPosX, float dropPosY)
    {
        // 0.0～1.0からの値をランダムで取得して
        // 指定割合以下ならアイテムドロップする
        if (Random.value > itemDropRatio) return;

        // ボートが居た場所にドロップする
        GameObject itemObj =
            (GameObject)Instantiate(
                itemHeart,
                new Vector3(dropPosX, dropPosY, 0.0f),
                Quaternion.identity);
        itemObj.SetActive(true);
    }

    // 爆発エフェクト生成
    public void CreateExplosionEffect(float posX, float posY)
    {
        float adjustY = 0.1f;   // 少し下側に表示する

        GameObject createObj =
            (GameObject)Instantiate(
                explosion,
                new Vector3(posX, posY - adjustY, 0.0f),
                Quaternion.identity);

        createObj.SetActive(true);  // 有効に
        ExplosionEffect expEffScript = createObj.GetComponent<ExplosionEffect>();

        // 爆発アニメ開始
        expEffScript.StartAnime();
    }

	// 削除されたボートをListから削除
	public void DeleteJapBoats()
	{
		for (int i = japBoats.Count -1; i >= 0; i--) {
			if (japBoats[i] == null) {
				japBoats.RemoveAt (i);
			}
		}
	}

}
