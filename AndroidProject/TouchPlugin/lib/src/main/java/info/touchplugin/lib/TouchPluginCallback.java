package info.touchplugin.lib;

import android.view.MotionEvent;
import com.unity3d.player.UnityPlayer;

public class TouchPluginCallback {
    private String mGameObject;
    private String mCallback;

    private String ACTION_BEGAN = "began";
    private String ACTION_ENDED = "ended";
    private String ACTION_MOVED = "moved";
    private String ACTION_CANCELLED = "cancelled";

    public TouchPluginCallback(String gameObject, String callback) {
        mGameObject = gameObject;
        mCallback = callback;
    }

    public void touchedCallback(MotionEvent event) {
        int action = event.getActionMasked();
        int pointerIndex = event.getActionIndex();
        String timeStamp = Long.toString(event.getEventTime());
        switch (action) {
            case MotionEvent.ACTION_POINTER_DOWN:
            case MotionEvent.ACTION_DOWN:
                sendMessage(ACTION_BEGAN + ",1, " + timeStamp + ","
                        + parseMotionEvent(pointerIndex, event));
                break;
            case MotionEvent.ACTION_POINTER_UP:
            case MotionEvent.ACTION_UP:
                sendMessage(ACTION_ENDED + ",1," + timeStamp + ","
                        + parseMotionEvent(pointerIndex, event));
                break;
            case MotionEvent.ACTION_CANCEL:
                sendMessage(ACTION_CANCELLED + ",1," + timeStamp + ","
                        + parseMotionEvent(pointerIndex, event));
                break;
            case MotionEvent.ACTION_MOVE:
                String param = "";
                for (int size = event.getPointerCount(), i = 0; i < size; i++) {
                    param += parseMotionEvent(i, event);
                }
                sendMessage(ACTION_MOVED + "," + Integer.toString(event.getPointerCount())
                        + "," + timeStamp + "," + param);
                break;
        }
    }

    private void sendMessage(String param) {
        UnityPlayer.UnitySendMessage(mGameObject, mCallback, param);
    }

    private String parseMotionEvent(int index, MotionEvent event) {
        String result = "";
        result += Integer.toString(event.getPointerId(index)) + ",";
        result += Float.toString(event.getX(index)) + ",";
        result += Float.toString(event.getY(index)) + ",";
        result += Float.toString(event.getSize(index)) + ",";
        result += Float.toString(event.getPressure(index)) + ",";
        return result;
    }
}
