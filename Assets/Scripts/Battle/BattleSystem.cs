// using System.Collections;
// using System.Collections.Generic;
// using JetBrains.Annotations;
// using Unity.VisualScripting;
// using UnityEditor.Search;
// using UnityEngine;
// using UnityEngine.PlayerLoop;

// public class BattleSystem : MonoBehaviour
// {
//     void Start()
//     {
//         InitializePlayer();
//         InitializeEnemies();
//         InitializeUI();

//         StartBattle();
//     }
// }

// #region Player
// public class Player : MonoBehaviour
// {
//     private PlayerStatsManager playerStatsManager;
//     private EquippedHerosManager equippedHerosManager;
//     private TimeManager timeManager;
//     private ItemManager itemManager;
//     private StatusEffectManager statusEffectManager;

//     void Start()
//     {
//         // 서버 저장된 내용 불러오기
//         // 모두 json으로 이루어진 파일로 초기화
//         InitializePlayerStats();
//         InitializeHero();
//         InitializeTime();
//         InitializeItem();

//         // 전투 끝나면 자체 초기화
//         InitializeStatusEffect();
//     }
// }

// // 자체 초기화
// // 상태 효과, 소울 게이지

// // 유지
// // PlayerStats(HP), Time, ItemCount

// // 체크해야할 점
// // 스테이지 돌입했으면 영웅 변경 불가창을 로비에서 띄워줘야 함

// public class JsonBattleStart
// {
//     public JsonPlayerStats playerStats;
//     public JsonEquippedHeros equippedHeros;
//     public JsonTime time;
//     public JsonEquippedItems items;
//     public JsonBattleResult battleResult;
// };

// public class JsonBattleResult
// {
//     public JsonBattleStatus battleStatus;
//     public List<string> rewards;
// }

// public class JsonBattleStatus
// {
//     public float currentHP;
//     public float currentTime;
//     public Dictionary<JsonItem, int> itemCount;
// }

// // 다양한 업그레이드로 증가된 HP
// public class JsonPlayerStats
// {
//     public float HP;
// }

// // 실제 HP 관리 함수
// public class PlayerStatsManager
// {
//     private float maxHP;
//     private float currentHP;

//     void Start()
//     {
//         // Json 변환
//         Initialize();
//     }
// }

// //Hero를 soulPrism별로 모아서 하나로 관리할지 고민
// // {
// //     Public string Name;
// //     Public List<JsonHeroData> HeroData;
// // }
// public class JsonHero
// {
//     public string heroID;
//     public string heroName;
//     public string description;
//     public SoulPrismType soulPrism;
//     public string equippedSkillID;
// };

// public class Hero
// {
//     public string heroID;
//     public string heroName;
//     public string description;
//     public SoulPrismType soulPrism;
//     public Skill skill;
// }

// // 로비에서 선택할 때 사용할듯
// public class JsonEquippedHeros
// {
//     public SoulPrismType soulPrism;
//     public SoulPrismCategory soulPrismCategory;
//     public List<JsonHero> Heros;
// }

// public class EquippedHerosManager
// {
//     public SoulPrismType soulPrism;
//     public SoulPrismCategory soulPrismCategory;
//     public List<Hero> Heros;

//     void Start()
//     {
//         // Json 변환
//         Initialize();
//     }
// }

// public class JsonTime
// {
//     public float time;
// }

// public class TimeManager
// {
//     private float maxTime;
//     private float currentTime;

//     void Start()
//     {
//         // Json 변환
//         Initialize();
//     }
// }

// public class JsonItem
// {
//     public string itemID;
//     public string itemName;
//     public ItemType itemType;
//     public float value;
//     public string description;
// }

// // 실제 아이템 효과
// public class Item
// {
//     void start()
//     {
//         initialize();
//     }
// }

// public class JsonEquippedItems
// {
//     public List<string> equippedItemIDs;
// }

// public class EquippedItemsManager
// {
//     private Dictionary<Item, int> itemCount;

//     void Start()
//     {
//         // Json 변환
//         Initialize();
//     }
// }


// //자체 초기화
// public class StatusEffectManager
// {
//     private List<Buff> buffs;
//     private List<Debuff> debuffs;
// }


// #endregion

// #region Skill



// #endregion

