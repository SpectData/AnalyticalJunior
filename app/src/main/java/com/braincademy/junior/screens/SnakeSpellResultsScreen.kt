package com.braincademy.junior.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.braincademy.junior.ui.theme.Background
import com.braincademy.junior.ui.theme.Purple60
import com.braincademy.junior.ui.theme.TextPrimary
import com.braincademy.junior.ui.theme.TextSecondary

@Composable
fun SnakeSpellResultsScreen(
    score: Int,
    wavesCompleted: Int,
    snakesKilled: Int,
    questionsAnswered: Int,
    questionsCorrect: Int,
    onPlayAgain: () -> Unit,
    onMenu: () -> Unit,
) {
    val accuracy =
        if (questionsAnswered > 0) {
            (questionsCorrect * 100) / questionsAnswered
        } else {
            0
        }

    Column(
        modifier =
            Modifier
                .fillMaxSize()
                .background(Background)
                .padding(20.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center,
    ) {
        Card(
            modifier = Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(16.dp),
            elevation = CardDefaults.cardElevation(6.dp),
        ) {
            Column(
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .padding(32.dp),
                horizontalAlignment = Alignment.CenterHorizontally,
            ) {
                Text(
                    "Game Over!",
                    fontSize = 28.sp,
                    fontWeight = FontWeight.Bold,
                    color = TextPrimary,
                )

                Spacer(Modifier.height(8.dp))

                Text(
                    "\uD83D\uDC0D\u2728",
                    fontSize = 48.sp,
                    textAlign = TextAlign.Center,
                )

                Spacer(Modifier.height(16.dp))

                Text(
                    "$score points",
                    fontSize = 32.sp,
                    fontWeight = FontWeight.Bold,
                    color = Purple60,
                )

                Spacer(Modifier.height(20.dp))

                StatRow("Waves Survived", "$wavesCompleted")
                StatRow("Snakes Defeated", "$snakesKilled")
                StatRow("Questions Answered", "$questionsAnswered")
                StatRow("Accuracy", "$accuracy%")

                Spacer(Modifier.height(24.dp))

                Row(
                    horizontalArrangement = Arrangement.spacedBy(12.dp),
                ) {
                    Button(
                        onClick = onPlayAgain,
                        colors = ButtonDefaults.buttonColors(containerColor = Purple60),
                        shape = RoundedCornerShape(12.dp),
                    ) {
                        Text("Play Again", fontWeight = FontWeight.SemiBold)
                    }
                    Button(
                        onClick = onMenu,
                        colors = ButtonDefaults.buttonColors(containerColor = Color(0xFFBDC3C7)),
                        shape = RoundedCornerShape(12.dp),
                    ) {
                        Text("Menu", fontWeight = FontWeight.SemiBold)
                    }
                }
            }
        }
    }
}

@Composable
private fun StatRow(
    label: String,
    value: String,
) {
    Row(
        modifier =
            Modifier
                .fillMaxWidth()
                .padding(vertical = 4.dp),
        horizontalArrangement = Arrangement.SpaceBetween,
    ) {
        Text(label, fontSize = 15.sp, color = TextSecondary)
        Text(value, fontSize = 15.sp, fontWeight = FontWeight.SemiBold, color = TextPrimary)
    }
}
