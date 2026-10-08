using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public static class GroupScoreService
    {
        public static int GetGroupScore(
            int listenersCount,
            int participantsCount,
            int superParticipantsCount,
            int listenerScore,
            int participantScore,
            int superParticipantScore,
            int studentsCount)
        {
            if (studentsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(studentsCount), "Количество студентов должно быть больше нуля.");
            }
            if (listenersCount < 0 || participantsCount < 0 || superParticipantsCount < 0)
            {
                throw new ArgumentOutOfRangeException("Количество участников не может быть отрицательным.");
            }

            double coverageRatio = (double)(listenersCount + participantsCount + superParticipantsCount) / studentsCount;

            double listenersScore = listenersCount * listenerScore * coverageRatio;
            double participantsScore = participantsCount * participantScore;
            double superParticipantsScore = superParticipantsCount * superParticipantScore;

            double totalScore = listenersScore + participantsScore + superParticipantsScore;

            return (int)Math.Round(totalScore, MidpointRounding.AwayFromZero);
        }
    }
}
