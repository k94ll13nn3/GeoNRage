namespace GeoNRage.Server;

public static class ExperienceEngine
{
    public static PlayerExperienceDto GetLevel(int experience)
    {
        int lastRemainingExperience = 0;
        int remainingExperience = experience;
        int level = 0;

        while (remainingExperience > 0)
        {
            lastRemainingExperience = remainingExperience;
            level++;
            remainingExperience -= GetExperienceForLevel(level);
        }

        level = Math.Max(level, 1);

        return new(level, experience, lastRemainingExperience, GetExperienceForLevel(level));

        static int GetExperienceForLevel(int l)
        {
            return 20 + (l * l);
        }
    }
}
