using UnityEngine;

namespace DontLookBehindYou.Managers
{
    public class GyroManager : MonoBehaviour
    {
        public static GyroManager Instance { get; private set; }
        
        [Header("Settings")]
        public bool isGyroEnabled = true;
        public float gyroSensitivity = 1.0f;

        private bool gyroSupported;
        private Gyroscope gyro;
        private Quaternion baseRotation;
        private Quaternion calibration;
        private bool isCalibrated = false;

        public Vector2 GyroDelta { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            CheckGyroSupport();
        }

        private void CheckGyroSupport()
        {
            gyroSupported = SystemInfo.supportsGyroscope;
            if (gyroSupported)
            {
                gyro = Input.gyro;
                gyro.enabled = true;
                
                // Wait for the next frame to calibrate
                Invoke(nameof(CalibrateGyro), 0.5f);
            }
            else
            {
                Debug.LogWarning("[GyroManager] Gyroscope not supported on this device.");
            }
        }

        public void ToggleGyro(bool enable)
        {
            isGyroEnabled = enable;
            if (gyroSupported && enable)
            {
                CalibrateGyro();
            }
        }

        public void CalibrateGyro()
        {
            if (!gyroSupported) return;
            
            calibration = Quaternion.Inverse(GyroToUnity(gyro.attitude));
            isCalibrated = true;
        }

        private void Update()
        {
            if (!gyroSupported || !isGyroEnabled || !isCalibrated)
            {
                GyroDelta = Vector2.zero;
                return;
            }

            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                GyroDelta = Vector2.zero;
                return;
            }

            // Calculate rotation change (delta) since last frame
            // A simpler approach for delta is to check rotational velocity
            Vector3 rotationRate = gyro.rotationRateUnbiased;
            
            // Map gyroscope rotation rate to our look input
            // gyro x axis maps to pitch (looking up/down), y maps to yaw (looking left/right)
            float mouseX = -rotationRate.y * gyroSensitivity;
            float mouseY = -rotationRate.x * gyroSensitivity;

            GyroDelta = new Vector2(mouseX, mouseY);
        }

        // Helper function to convert Android Gyro coordinates to Unity coordinates
        private Quaternion GyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }
}
