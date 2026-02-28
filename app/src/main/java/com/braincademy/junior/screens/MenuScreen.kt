package com.braincademy.junior.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.braincademy.junior.model.Category
import com.braincademy.junior.model.GameStats
import com.braincademy.junior.ui.theme.Background
import com.braincademy.junior.ui.theme.LogicOrange
import com.braincademy.junior.ui.theme.MathBlue
import com.braincademy.junior.ui.theme.Purple60
import com.braincademy.junior.ui.theme.Purple80
import com.braincademy.junior.ui.theme.SnakeGreen
import com.braincademy.junior.ui.theme.TextPrimary
import com.braincademy.junior.ui.theme.TextSecondary

@Composable
fun MenuScreen(
    stats: GameStats,
    onCategorySelected: (Category) -> Unit,
    onSnakeSpellSelected: () -> Unit = {},
) {
    Column(
        modifier =
            Modifier
                .fillMaxSize()
                .background(Background),
    ) {
        // Header
        Box(
            modifier =
                Modifier
                    .fillMaxWidth()
                    .background(Brush.linearGradient(listOf(Purple60, Purple80)))
                    .padding(vertical = 28.dp, horizontal = 20.dp),
            contentAlignment = Alignment.Center,
        ) {
            Column(horizontalAlignment = Alignment.CenterHorizontally) {
                Text(
                    "BRAIN ACADEMY",
                    color = Color.White,
                    fontSize = 26.sp,
                    fontWeight = FontWeight.Bold,
                    letterSpacing = 2.sp,
                )
                Text(
                    "Train your brain, level up your mind!",
                    color = Color.White.copy(alpha = 0.85f),
                    fontSize = 14.sp,
                )
            }
        }

        // Score bar
        Card(
            modifier =
                Modifier
                    .fillMaxWidth()
                    .padding(horizontal = 20.dp, vertical = 12.dp),
            shape = RoundedCornerShape(12.dp),
            elevation = CardDefaults.cardElevation(2.dp),
        ) {
            Row(
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                horizontalArrangement = Arrangement.SpaceEvenly,
            ) {
                StatItem("TOTAL SCORE", stats.totalScore.toString())
                StatItem("GAMES PLAYED", stats.gamesPlayed.toString())
                StatItem("BEST STREAK", stats.bestStreak.toString())
            }
        }

        Spacer(Modifier.height(16.dp))

        Text(
            "Choose a Category",
            fontSize = 20.sp,
            fontWeight = FontWeight.Bold,
            color = TextPrimary,
            textAlign = TextAlign.Center,
            modifier = Modifier.fillMaxWidth(),
        )

        Spacer(Modifier.height(16.dp))

        // Category cards
        Column(
            modifier = Modifier.padding(horizontal = 20.dp),
            verticalArrangement = Arrangement.spacedBy(14.dp),
        ) {
            CategoryCard(
                icon = "\uD83D\uDD22",
                title = "Math",
                description = "Quick calculations, number sequences, and comparisons",
                accentColor = MathBlue,
                onClick = { onCategorySelected(Category.MATH) },
            )
            CategoryCard(
                icon = "\uD83E\uDDE9",
                title = "Logic & Puzzles",
                description = "Pattern recognition, memory, and odd-one-out challenges",
                accentColor = LogicOrange,
                onClick = { onCategorySelected(Category.LOGIC) },
            )
            CategoryCard(
                icon = "\uD83D\uDC0D\u2728",
                title = "Snake Spellcaster",
                description = "Cast spells at snakes by solving math! Tower defense meets brain training.",
                accentColor = SnakeGreen,
                onClick = onSnakeSpellSelected,
            )
        }
    }
}

@Composable
private fun StatItem(
    label: String,
    value: String,
) {
    Column(horizontalAlignment = Alignment.CenterHorizontally) {
        Text(label, fontSize = 10.sp, color = TextSecondary, letterSpacing = 1.sp)
        Text(value, fontSize = 22.sp, fontWeight = FontWeight.Bold, color = TextPrimary)
    }
}

@Composable
private fun CategoryCard(
    icon: String,
    title: String,
    description: String,
    accentColor: Color,
    onClick: () -> Unit,
) {
    Card(
        modifier =
            Modifier
                .fillMaxWidth()
                .clip(RoundedCornerShape(16.dp))
                .clickable(onClick = onClick),
        shape = RoundedCornerShape(16.dp),
        elevation = CardDefaults.cardElevation(4.dp),
    ) {
        Column {
            // Accent top border
            Box(
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .height(5.dp)
                        .background(accentColor),
            )
            Column(modifier = Modifier.padding(20.dp)) {
                Text(icon, fontSize = 36.sp)
                Spacer(Modifier.height(8.dp))
                Text(title, fontSize = 18.sp, fontWeight = FontWeight.SemiBold, color = TextPrimary)
                Spacer(Modifier.height(4.dp))
                Text(description, fontSize = 13.sp, color = TextSecondary)
            }
        }
    }
}
