using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeCtrl : MonoBehaviour
{

    public Image coolTimeImage;
    public Text coolTimeCounter; //남은 쿨타임을 표시할 텍스트

    public float itemCoolTime;
    public float reloadCoolTime;

    private float currentCoolTime; //남은 쿨타임을 추적 할 변수

    private bool reStartFunction = true; // 재장전이나 아이템을 사용할 수 있는지 확인하는 변수

    public PlayerCtrl player;

    void start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        //gameObject.SetActive(false);
        //coolTimeImage.fillAmount = 0; //처음에 스킬 버튼을 가리지 않음
    }

    public void UseItemCollTime()
    {
        gameObject.SetActive(true);
        if (reStartFunction)
        {
            Debug.Log("Use Item");

            // 쿨타임 버튼 채우기
            coolTimeImage.fillAmount = 1;

            StartCoroutine("HpCooltime");

            currentCoolTime = itemCoolTime;
            coolTimeCounter.text = "" + currentCoolTime;

            StartCoroutine("HpCoolTimeCounter");

            reStartFunction = false; // 실행 되면 사용하면 사용할 수 없는 상태로 바꿈
        }
    }

    public void ReloadCollTime()
    {
        gameObject.SetActive(true);
        if (reStartFunction)
        {
            Debug.Log("Reload Bullet");

            // 쿨타임 버튼 채우기
            coolTimeImage.fillAmount = 1;

            StartCoroutine("ReloadCooltime");

            currentCoolTime = reloadCoolTime;
            coolTimeCounter.text = "" + currentCoolTime;

            StartCoroutine("ReloadCoolTimeCounter");



            reStartFunction = false; //스킬을 사용하면 사용할 수 없는 상태로 바꿈
        }
    }

    IEnumerator HpCooltime()
    {
        while (coolTimeImage.fillAmount > 0)
        {
            coolTimeImage.fillAmount -= 1 * Time.smoothDeltaTime / itemCoolTime;

            yield return null;
        }

        reStartFunction = true; //스킬 쿨타임이 끝나면 스킬을 사용할 수 있는 상태로 바꿈

        yield break;
    }

    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator HpCoolTimeCounter()
    {
        while (currentCoolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCoolTime -= 1.0f;
            coolTimeCounter.text = "" + currentCoolTime;
        }
        if (player.hp <= 30)
        {
            player.hp += 70;
            player.send_PlayerHP(player.hp, player.armour);
        }
        else if (player.hp > 30)
        {
            player.hp = 100;
            player.send_PlayerHP(player.hp, player.armour);
        }
        player.imgHpBar.fillAmount = (float)player.hp / (float)player.initHp;
        gameObject.SetActive(false);
        yield break;
    }


    IEnumerator ReloadCooltime()
    {
        while (coolTimeImage.fillAmount > 0)
        {
            coolTimeImage.fillAmount -= 1 * Time.smoothDeltaTime / reloadCoolTime;

            yield return null;
        }

        reStartFunction = true; //스킬 쿨타임이 끝나면 스킬을 사용할 수 있는 상태로 바꿈

        yield break;
    }

    //남은 쿨타임을 계산할 코르틴을 만들어줍니다.
    IEnumerator ReloadCoolTimeCounter()
    {
        while (currentCoolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCoolTime -= 1.0f;
            coolTimeCounter.text = "" + currentCoolTime;
        }

        player.ReloadBullet();
        gameObject.SetActive(false);
        yield break;
    }
}

