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
  [ScoreCategory.Ones]: 'Ones',
  [ScoreCategory.Twos]: 'Twos',
  [ScoreCategory.Threes]: 'Threes',
  [ScoreCategory.Fours]: 'Fours',
  [ScoreCategory.Fives]: 'Fives',
  [ScoreCategory.Sixes]: 'Sixes',
  [ScoreCategory.OnePair]: 'One Pair',
  [ScoreCategory.TwoPairs]: 'Two Pairs',
  [ScoreCategory.ThreeOfAKind]: 'Three of a Kind',
  [ScoreCategory.FourOfAKind]: 'Four of a Kind',
  [ScoreCategory.SmallStraight]: 'Small Straight',
  [ScoreCategory.LargeStraight]: 'Large Straight',
  [ScoreCategory.FullHouse]: 'Full House',
  [ScoreCategory.Chance]: 'Chance',
  [ScoreCategory.Yatzy]: 'Yatzy'
};
