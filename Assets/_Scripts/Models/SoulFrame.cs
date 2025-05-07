namespace AiGameProject.Models
{
    public enum SoulFrameType
    {
        INTP, INTJ, ENTJ, ENTP,
        INFJ, INFP, ENFJ, ENFP,
        ISTJ, ISFJ, ESTJ, ESFJ,
        ISTP, ISFP, ESTP, ESFP
    }

    public class SoulFrame
    {
        public SoulFrameType Type { get; private set; }

        public SoulFrame(SoulFrameType type)
        {
            Type = type;
        }

        public string GetEmotionReaction()
        {
            return Type switch
            {
                SoulFrameType.ENFP => "아군이 쓰러질 때 감정 고양",
                SoulFrameType.INFP => "부정적 감정에 쉽게 흔들림",
                SoulFrameType.ENTP => "반박/도전 시 감정 상승",
                SoulFrameType.INTP => "감정 변동 거의 없음",
                SoulFrameType.ESFP => "감정 이벤트에 즉시 반응",
                SoulFrameType.ISFP => "타인 감정에 민감",
                SoulFrameType.ESTP => "통제받으면 광기 상승",
                SoulFrameType.ISTP => "감정 변화 거의 없음",
                SoulFrameType.ENFJ => "동료 위기 시 감정 상승",
                SoulFrameType.INFJ => "억제형, 누적 빠름",
                SoulFrameType.ENTJ => "통제 실패 시 격렬 반응",
                SoulFrameType.INTJ => "내면 분노 누적",
                SoulFrameType.ESFJ => "감정 공명 과몰입",
                SoulFrameType.ISFJ => "감정 고갈로 도피",
                SoulFrameType.ESTJ => "질서 파괴에 분노",
                SoulFrameType.ISTJ => "변화 반복 시 불안",
                _ => "중립"
            };
        }

        public string GetMadnessBehavior()
        {
            return Type switch
            {
                SoulFrameType.ENFP => "전장을 무대처럼 착각",
                SoulFrameType.INFP => "극단적 희생",
                SoulFrameType.ENTP => "무의미한 논쟁 반복",
                SoulFrameType.INTP => "무기력하고 단절된 행동",
                SoulFrameType.ESFP => "자극 과잉 상태",
                SoulFrameType.ISFP => "무기력 or 자해적 선택지",
                SoulFrameType.ESTP => "충동적 돌진, 아군도 공격",
                SoulFrameType.ISTP => "기능 몰입 → 반복 행동",
                SoulFrameType.ENFJ => "자기희생적 폭주",
                SoulFrameType.INFJ => "비전 강박으로 현실 단절",
                SoulFrameType.ENTJ => "강박적 지시 반복",
                SoulFrameType.INTJ => "질서 강요 및 지배욕 발현",
                SoulFrameType.ESFJ => "과보호 → 아군 통제",
                SoulFrameType.ISFJ => "도피 or 극단적 순응",
                SoulFrameType.ESTJ => "규율 위반 아군에 적대",
                SoulFrameType.ISTJ => "현실 고정, 루틴만 반복",
                _ => "불명"
            };
        }
    }
}