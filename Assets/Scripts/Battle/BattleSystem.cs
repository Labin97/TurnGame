// using System.Collections;
// using System.Collections.Generic;
// using JetBrains.Annotations;
// using Unity.Mathematics;
// using Unity.VisualScripting;
// using UnityEditor.Search;
// using UnityEngine;
// using UnityEngine.PlayerLoop;


// //전투 시작될 때 발동
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
//     private TimeManager timeManager;
//     private ItemManager itemManager;
//     private StatusEffectManager statusEffectManager;
//     private EquippedHerosManager equippedHerosManager;

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
//         InitializeEquippedHerosManager();
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
//     public JsonEquippedItems equippedItems;
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

// // 다양한 업그레이드로 증가된 HP (로비에서 쓸 듯)
// public class JsonPlayerStats
// {
//     public float HP;
// }

// // 실제 HP 관리 함수 (배틀)
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
//     public string NormalSkillID;
//     public string SoulSkillID;
// };

// public class Hero
// {
//     public string heroID;
//     public string heroName;
//     public string description;
//     public SoulPrismType soulPrism;
//     public Skill normalSkill;
//     public Skill soulSkill;
// }

// // 로비에서 선택할 때 사용할듯
// public class JsonEquippedHeros
// {
//     public SoulPrismType soulPrism;
//     public SoulPrismCategory soulPrismCategory;
//     public JsonHero[] Heros = new JsonHero[4];
// }

// public class EquippedHerosManager
// {
//     private SoulPrismType soulPrism;
//     private SoulPrismCategory soulPrismCategory;
//     private Hero[] Heros = new Hero[4];
//     private float[] currentSoulGauges = new float[4];


//     private SkillQueue skillQueue;

//     void Start()
//     {
//         // Json 변환
//         Initialize();
//     }

//     // Hero의 스킬 사용을 대신 처리
//     public void UseHeroNormalSkill(int heroIndex)
//     {
//         if (heroIndex < 0 || heroIndex >= Heros.Length) return;

//         Hero hero = Heros[heroIndex];
//         Skill skill = hero.normalSkill;

//         if (timeManager.CanUseSkill(skill.TimeMinus))
//         {
//             timeManager.ConsumeTime(skill.TimeMinus);
//             skillQueue.EnqueueSkill(skill);
//         }
//     }

//     public void UseHeroSoulSkill(int heroIndex)
//     {
//         if (heroIndex < 0 || heroIndex >= Heros.Length) return;

//         Hero hero = Heros[heroIndex];
//         Skill skill = hero.soulSkill;

//         if (timeManager.CanUseSkill(skill.TimeMinus))
//         {
//             timeManager.ConsumeTime(skill.TimeMinus);
//             skillQueue.EnqueueSkill(skill, heroIndex);
//         }
//     }

//     public void AddSoulGauge(float amount, int heroIndex)
//     {
//         if (heroIndex < 0 || heroIndex >= Heros.Length) return;

//         currentSoulGauges[heroIndex] = math.clamp(currentSoulGauges[heroIndex] + amount,
//             0, Heros[heroIndex].normalSkill.SoulGaugeRequired);

//         //UI 업데이트
//     }
// }

// //currentTime은 따로 result로 관리
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

// // 타입별 실제 아이템 효과
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

// public class JsonSkill
// {
//     [Header("Basic Info")]
//     public string skillID;
//     public string skillName;
//     public SkillType skillType;
//     public SoulType soulType;
//     public string description;

//     [Header("Path")]
//     public string iconPath;
//     public string soundPath;
//     public string animationPath;

//     [Header("Damage Stats")]
//     public float baseValue;

//     [Header("Critical Stats")]
//     public float critChance;
//     public float critMultiplier;

//     [Header("Time Control")]
//     public float timePlus; // 스킬 사용시 타임 게이지 증가량
//     public float timeMinus; // 시간게이지 소모량

//     [Header("Soul Gauge")]
//     public float soulGaugeRequired = 5f; // 소울 스킬 발동에 필요한 횟수

//     [Header("Other Skill")]
//     public string[] otherSkillIDs; // 다른 스킬 ID 배열
// }


// public class Skill
// {
//     [Header("Basic Info")]
//     private string skillID;
//     private string skillName;
//     private SkillType skillType;
//     private SoulType soulType;
//     private string description;

//     [Header("Resources")]
//     private Sprite SkillIcon;
//     private AudioClip SkillSound;
//     private AnimationClip SkillAnimation;

//     [Header("Damage Stats")]
//     private float baseValue;

//     [Header("Critical Stats")]
//     private float critChance;
//     private float critMultiplier;

//     [Header("Time Control")]
//     private float timePlus; // 스킬 사용시 타임 게이지 증가량
//     private float timeMinus; // 시간게이지 소모량

//     [Header("Soul Gauge")]
//     private float soulGaugeRequired = 5; // 소울 스킬 발동에 필요한 횟수

//     [Header("Other Skill")]
//     private Skill[] otherSkills; // 다른 스킬 배열

//     public float SoulGaugeRequired => soulGaugeRequired;

//     private void Start()
//     {
//         //전투 시작시 currentSoulGauge 초기화
//         initialize();
//     }

//     public bool HasOtherSkills()
//     {
//         return otherSkills != null && otherSkills.Length > 0;
//     }
// }

// public class SkillQueue
// {
//     private Queue<Skill> skillQueue;

//     [Header("Runtime Values")]
//     private float currentValue;
//     private float currentCritChance;
//     private float currentCritMultiplier;
//     private float currentValueMultiplier;
//     private float soulGaugeGenerated;

//     public void EnqueueSkill(Skill skill)
//     {
//         skillQueue.Enqueue(skill);
//     }

//     public void ExecuteSkillQueue()
//     {
//         while (skillQueue.Count > 0)
//         {
//             Skill nextSkill = skillQueue.Dequeue();
//             ExecuteSkill(nextSkill);
//         }
//     }

//     private void ExecuteSkill(Skill skill)
//     {
//         RestAllRuntimeSkillValue();
//         BeforeActionOtherSkills();

//         CalculateValue();
//         AnimationStart();
//         // 애니메이션 시작과 동시에, 애니메이션 시간에 비례하여 timePlus 만큼 증가
//         PlusCurrentTime();
//     }

//     //애니메이션 타이밍에 맞춰 데미지나 버프가 들어가야함
//     private void AnimationStart()
//     {

//     }
// }
// #endregion

