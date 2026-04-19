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
  [ScoreCategory.Ones]: 'Ettere',
  [ScoreCategory.Twos]: 'Toere',
  [ScoreCategory.Threes]: 'Treere',
  [ScoreCategory.Fours]: 'Firere',
  [ScoreCategory.Fives]: 'Femmere',
  [ScoreCategory.Sixes]: 'Seksere',
  [ScoreCategory.OnePair]: 'Et par',
  [ScoreCategory.TwoPairs]: 'To par',
  [ScoreCategory.ThreeOfAKind]: 'Tre ens',
  [ScoreCategory.FourOfAKind]: 'Fire ens',
  [ScoreCategory.SmallStraight]: 'Lille straight',
  [ScoreCategory.LargeStraight]: 'Stor straight',
  [ScoreCategory.FullHouse]: 'Full house',
  [ScoreCategory.Chance]: 'Chance',
  [ScoreCategory.Yatzy]: 'Yatzy'
};
