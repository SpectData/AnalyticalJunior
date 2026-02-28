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
fun ResultsScreen(
    score: Int,
    correctCount: Int,
    totalQuestions: Int,
    accuracy: Int,
    starCount: Int,
    onPlayAgain: () -> Unit,
    onMenu: () -> Unit,
) {
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
                        .padding(36.dp),
                horizontalAlignment = Alignment.CenterHorizontally,
            ) {
                Text(
                    "Round Complete!",
                    fontSize = 24.sp,
                    fontWeight = FontWeight.Bold,
                    color = TextPrimary,
                )

                Spacer(Modifier.height(16.dp))

                // Stars
                val starText = "\u2B50".repeat(starCount) + "\u2606".repeat(3 - starCount)
                Text(
                    starText,
                    fontSize = 40.sp,
                    textAlign = TextAlign.Center,
                )

                Spacer(Modifier.height(16.dp))

                Text(
                    "$score points",
                    fontSize = 32.sp,
                    fontWeight = FontWeight.Bold,
                    color = Purple60,
                )

                Spacer(Modifier.height(8.dp))

                Text(
                    "$correctCount / $totalQuestions correct",
                    fontSize = 15.sp,
                    color = TextSecondary,
                )
                Text(
                    "$accuracy% accuracy",
                    fontSize = 15.sp,
                    color = TextSecondary,
                )

                Spacer(Modifier.height(28.dp))

                Row(
                    horizontalArrangement = Arrangement.spacedBy(12.dp),
                ) {
                    Button(
                        onClick = onPlayAgain,
                        colors =
                            ButtonDefaults.buttonColors(
                                containerColor = Purple60,
                            ),
                        shape = RoundedCornerShape(12.dp),
                    ) {
                        Text("Play Again", fontWeight = FontWeight.SemiBold)
                    }
                    Button(
                        onClick = onMenu,
                        colors =
                            ButtonDefaults.buttonColors(
                                containerColor = Color(0xFFBDC3C7),
                            ),
                        shape = RoundedCornerShape(12.dp),
                    ) {
                        Text("Menu", fontWeight = FontWeight.SemiBold)
                    }
                }
            }
        }
    }
}
