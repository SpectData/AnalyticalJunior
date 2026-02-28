"""Write diagram-dependent txt files for maths P3 questions (batch 2)."""
import os

outdir = os.path.join(os.path.dirname(__file__), "..", "docs", "maths", "raw", "p3")

questions = {}

questions[14] = """Q14

[Diagram: Two shapes drawn on a dot grid (dots spaced 1 cm apart). Shape P is a small rectangle spanning 2 units wide x 1 unit tall (area = 2 cm squared). Shape Q is a large rectangular shape approximately 4 units wide x 5 units tall, with a 2x1 notch cut from the top-left corner, drawn on the same dot grid.]

Shape P has an area of 2 cm squared.
What is the area of Shape Q?

A 8 cm squared
B 10 cm squared
C 15 cm squared
D 16 cm squared
E 24 cm squared
"""

questions[17] = """Q17

[Diagram: A square fence with side length 4 m. Posts are placed 2 m apart along each side, with a post at each corner. The diagram shows 8 posts: 3 on each side, with corner posts shared between adjacent sides.]

Selina makes fences around square pieces of land.
Each fence has posts that are 2 metres apart. There is a post at each corner of the square.
To make a square with side length 4 m, she needs 8 posts as shown in the picture.
How many posts does she need to make a square with side length 10 m?

A 16
B 20
C 24
D 25
E 36
"""

questions[18] = """Q18

[Diagram: Two thermometers side by side, scaled 0-30 degrees C. The outside thermometer shows approximately 4.5 degrees C. The inside thermometer shows approximately 21 degrees C.]

These thermometers show the temperature outside and inside.
What is the difference between the temperature outside and the temperature inside?

A 15 degrees C
B 16 degrees C
C 16 1/2 degrees C
D 17 degrees C
E 17 1/2 degrees C
"""

questions[19] = """Q19

[Diagram: Three rectangular building blocks shown in 3D with labelled dimensions:
Block 1: 3 cm x 5 cm x 4 cm
Block 2: 4 cm x 4 cm x 5 cm
Block 3: 7 cm x 6 cm x 5 cm
Marked 'diagram not to scale'.]

Jake has three different building blocks.
He stacks all three on top of each other in different ways.
What is the tallest stack he can make?

A 13 cm
B 14 cm
C 15 cm
D 17 cm
E 18 cm
"""

questions[20] = """Q20

[Diagram: Two identical squares, X and Y.
Square X is divided into an 8x8 grid (64 small squares). A small number of squares (approximately 8) are shaded in a cluster near the upper-middle area.
Square Y is divided into 8 equal triangles by its two diagonals and two medians. Three of the 8 triangles are shaded (forming a wedge on one side).]

Myo has two identical squares, X and Y.
She divides square X into 64 equal-sized small squares.
She divides square Y into 8 equal-sized triangles.
She shades some squares on X and some triangles on Y, as shown.
How many more small squares must Myo shade on X so that the same area is shaded on both diagrams?

A 5
B 8
C 16
D 22
E 24
"""

questions[21] = """Q21

[Diagram: Five polyomino shapes labelled A-E on grid paper:
Shape A: A 6-wide x 3-tall rectangle (18 squares)
Shape B: A 5-wide x 2-tall rectangle (10 squares)
Shape C: An L-shaped piece roughly 4x2 with a notch (approximately 6 squares)
Shape D: An L-shaped piece roughly 3x3 with a corner removed (approximately 7 squares)
Shape E: A tiny 2x1 rectangle (2 squares)
Below them is an empty 6x6 grid (36 squares total).]

Fran has one each of the shapes below, which she can move and rotate.
She has an empty 6 by 6 grid.
Fran realises she can cover the grid exactly using four of these shapes, with no shapes overlapping.
Which shape is not used?

A shape A
B shape B
C shape C
D shape D
E shape E
"""

questions[22] = """Q22

[Diagram: Six number cards in a row: 8, diamond, 4, 5, hexagon, 2. The diamond (light shape) and hexagon (dark shape) represent two unknown whole numbers greater than zero.]

The sum of the six number cards below is 25.
The diamond and hexagon represent two whole numbers that are greater than zero.
The cards are: 8, diamond, 4, 5, hexagon, 2.
What is the largest possible difference between the diamond and the hexagon?

A 0
B 2
C 3
D 4
E 6
"""

questions[23] = """Q23

[Diagram: A vertical container with a scale from 0 (bottom) to 40 mL (top). The scale has evenly spaced tick marks. The water level (shown by shading) is at approximately 60 percent of the way from bottom to top. The container has straight sides.]

Birrani is measuring water in a container. This container has an unusual scale.
What is the volume of water Birrani has?

A 24 mL
B 25 mL
C 26 mL
D 28 mL
E 32 mL
"""

questions[27] = """Q27

[Diagram: A column graph (without scale markings on the y-axis) showing two bars for Phil and Suzie. Suzie's bar is taller than Phil's, approximately in a 5:4 ratio.
A picture graph shows Phil's row with: two full tyre symbols (4 tyres each) and one 3/4 tyre symbol (3 tyres) = 11 tyres total. Suzie's row is empty.
Key: full tyre = 4, 3/4 tyre = 3, half tyre = 2, quarter tyre = 1.]

The column graph shows the number of car tyres replaced by two mechanics, Phil and Suzie, in one day. The scale is missing from the graph.
The picture graph shows the number of tyres replaced by Phil. The pictures for Suzie are missing.
What should the picture graph show for Suzie?

A [two full + 3/4 tyre = 11 tyres]
B [three full tyres = 12 tyres]
C [three full + 1/4 tyre = 13 tyres]
D [three full + 3/4 tyre = 15 tyres]
E [four full + 3/4 tyre = 19 tyres]
"""

questions[29] = """Q29

[Diagram: A number line showing whole numbers 7, 8, and 9 with minor tick marks dividing each unit into 4 equal parts (quarters). An arc/arrow shows one jump going backwards (left) from 9 by 1/4 of a unit.]

Here is part of a number line.
The arrow represents one jump.
Starting from 9, what value will be reached on the number line after 11 of these jumps?

A 6 1/4
B 6 1/2
C 6 3/4
D 7 1/4
E 7 3/4
"""

questions[30] = """Q30

Here is a bus timetable:

Forster to Coffs Harbour:
              Bus 1      Bus 2
Forster       7:00 am    8:50 am
Wauchope      8:48 am    10:38 am
Kempsey       9:58 am    11:48 am
Coffs Harbour 11:49 am   1:39 pm

Coffs Harbour to Forster:
              Bus 3      Bus 4
Coffs Harbour 2:39 pm    4:25 pm
Kempsey       4:40 pm    6:26 pm
Wauchope      5:53 pm    7:39 pm
Forster       7:33 pm    9:19 pm

Sarah and her friends live in Forster and take the 8:50 am bus to Coffs Harbour.
They stay in Coffs Harbour until they get a bus home, arriving in Forster at 9:19 pm that evening.
How long do they spend at Coffs Harbour?

A 2 hours 46 minutes
B 3 hours 14 minutes
C 3 hours 26 minutes
D 4 hours 36 minutes
E 7 hours 40 minutes
"""

questions[34] = """Q34

[Diagram: A 7-sided polygon (heptagon) with three dashed lines cutting through it. One dashed line cuts vertically near the left side, one cuts diagonally from lower-left to lower-right, and one cuts diagonally from upper area to the right. The cuts create four pieces: three triangles and one quadrilateral.]

The diagram shows a 7-sided shape.
It is cut along the three dashed lines shown. This makes four pieces: three triangles and one other shape.
How many sides do the four pieces have in total?

A 11
B 13
C 14
D 16
E 17
"""

questions[35] = """Q35

[Diagram: A map of Aboriginal regions in eastern Australia. North is up. Tamworth is marked in the KAMILAROI region (lower-centre). Brisbane is marked in the YUGGERA region (upper-right). The regions from south to north include:
Bottom row: WONNARUA, GEAWEGAL
Lower-middle: KAMILAROI (contains Tamworth), BIRIPI, DHANGATTI
Middle: NGANYAY-WANA, NGARABAL, GUMBAYNGGIRR
Upper-middle: BIGAMBUL, BUNDJALUNG
Top row: MANDANDANJI, BARUNGGAM, WAKA WAKA, GUBBI, YUGGERA (contains Brisbane)]

The map shows some of the Aboriginal regions of eastern Australia.
Shuyi plans a route from Tamworth to Brisbane. On her route she only travels north or east. She can make as many turns as she wants, but always travels north or east.
What is the greatest number of different regions she could pass through? (Do not include Kamilaroi or Yuggera.)

A 3
B 5
C 6
D 7
E 8
"""

for qnum, content in questions.items():
    path = os.path.join(outdir, f"p3q{qnum}.txt")
    with open(path, "w", encoding="utf-8") as f:
        f.write(content.strip() + "\n")
    print(f"Written: p3q{qnum}.txt")

print(f"\nDone: {len(questions)} diagram questions (batch 2).")
