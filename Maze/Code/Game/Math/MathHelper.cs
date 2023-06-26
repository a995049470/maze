using Stride.Core.Mathematics;
using System;

namespace Maze.Code.Game
{
    public static class MathHelper 
    {
        

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime,  float maxSpeed,  float deltaTime)
        {
            float output_x = 0f;
            float output_y = 0f;
            float output_z = 0f;

            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = MathF.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);

            float change_x = current.X - target.X;
            float change_y = current.Y - target.Y;
            float change_z = current.Z - target.Z;
            Vector3 originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;

            float maxChangeSq = maxChange * maxChange;
            float sqrmag = change_x * change_x + change_y * change_y + change_z * change_z;
            if (sqrmag > maxChangeSq)
            {
                var mag = (float)MathF.Sqrt(sqrmag);
                change_x = change_x / mag * maxChange;
                change_y = change_y / mag * maxChange;
                change_z = change_z / mag * maxChange;
            }

            target.X = current.X - change_x;
            target.Y = current.Y - change_y;
            target.Z = current.Z - change_z;

            float temp_x = (currentVelocity.X + omega * change_x) * deltaTime;
            float temp_y = (currentVelocity.Y + omega * change_y) * deltaTime;
            float temp_z = (currentVelocity.Z + omega * change_z) * deltaTime;

            currentVelocity.X = (currentVelocity.X - omega * temp_x) * exp;
            currentVelocity.Y = (currentVelocity.Y - omega * temp_y) * exp;
            currentVelocity.Z = (currentVelocity.Z - omega * temp_z) * exp;

            output_x = target.X + (change_x + temp_x) * exp;
            output_y = target.Y + (change_y + temp_y) * exp;
            output_z = target.Z + (change_z + temp_z) * exp;

            // Prevent overshooting
            float origMinusCurrent_x = originalTo.X - current.X;
            float origMinusCurrent_y = originalTo.Y - current.Y;
            float origMinusCurrent_z = originalTo.Z - current.Z;
            float outMinusOrig_x = output_x - originalTo.X;
            float outMinusOrig_y = output_y - originalTo.Y;
            float outMinusOrig_z = output_z - originalTo.Z;

            if (origMinusCurrent_x * outMinusOrig_x + origMinusCurrent_y * outMinusOrig_y + origMinusCurrent_z * outMinusOrig_z > 0)
            {
                output_x = originalTo.X;
                output_y = originalTo.Y;
                output_z = originalTo.Z;

                currentVelocity.X = (output_x - originalTo.X) / deltaTime;
                currentVelocity.Y = (output_y - originalTo.Y) / deltaTime;
                currentVelocity.Z = (output_z - originalTo.Z) / deltaTime;
            }

            return new Vector3(output_x, output_y, output_z);
        }
    }
}
