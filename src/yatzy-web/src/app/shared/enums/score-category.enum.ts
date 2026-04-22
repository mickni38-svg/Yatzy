export enum ScoreCategory {
  Ones = 'Ones',
  Twos = 'Twos',
  Threes = 'Threes',
  Fours = 'Fours',
  Fives = 'Fives',
  Sixes = 'Sixes',
  OnePair = 'OnePair',
  TwoPairs = 'TwoPairs',
  ThreeOfAKind = 'ThreeOfAKind',
  FourOfAKind = 'FourOfAKind',
  SmallStraight = 'SmallStraight',
  LargeStraight = 'LargeStraight',
  FullHouse = 'FullHouse',
  Chance = 'Chance',
  Yatzy = 'Yatzy'
}

export const ScoreCategoryLabel: Record<ScoreCategory, string> = {
  [ScoreCategory.Ones]: "1' ere",
  [ScoreCategory.Twos]: "2' ere",
  [ScoreCategory.Threes]: "3' ere",
  [ScoreCategory.Fours]: "4' ere",
  [ScoreCategory.Fives]: "5' ere",
  [ScoreCategory.Sixes]: "6' ere",
  [ScoreCategory.OnePair]: '1 par',
  [ScoreCategory.TwoPairs]: '2 par',
  [ScoreCategory.ThreeOfAKind]: '3 ens',
  [ScoreCategory.FourOfAKind]: '4 ens',
  [ScoreCategory.SmallStraight]: 'Lille straight',
  [ScoreCategory.LargeStraight]: 'Stor straight',
  [ScoreCategory.FullHouse]: 'Hus',
  [ScoreCategory.Chance]: 'Chance',
  [ScoreCategory.Yatzy]: 'YATZY'
};

export const ScoreCategoryMax: Record<ScoreCategory, number> = {
  [ScoreCategory.Ones]: 5,
  [ScoreCategory.Twos]: 10,
  [ScoreCategory.Threes]: 15,
  [ScoreCategory.Fours]: 20,
  [ScoreCategory.Fives]: 25,
  [ScoreCategory.Sixes]: 30,
  [ScoreCategory.OnePair]: 12,
  [ScoreCategory.TwoPairs]: 22,
  [ScoreCategory.ThreeOfAKind]: 18,
  [ScoreCategory.FourOfAKind]: 24,
  [ScoreCategory.SmallStraight]: 15,
  [ScoreCategory.LargeStraight]: 20,
  [ScoreCategory.FullHouse]: 28,
  [ScoreCategory.Chance]: 30,
  [ScoreCategory.Yatzy]: 50
};
