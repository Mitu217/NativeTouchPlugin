package info.touchplugin.lib;

import android.graphics.Color;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.LinearLayout;

import com.unity3d.player.UnityPlayer;

public class TouchPlugin {
    private static TouchPluginCallback mCallBack;
    private static UnityPlayer mUnityPlayer;


    public static void initPlugin(String gameObject, String callback) {
        mCallBack = new TouchPluginCallback(gameObject, callback);

        ViewGroup rootView = (ViewGroup)UnityPlayer.currentActivity.getWindow().getDecorView().findViewById(android.R.id.content);
        for (int n=0; n<rootView.getChildCount(); n++) {
          if(rootView.getChildAt(n) instanceof UnityPlayer) {
              mUnityPlayer = (UnityPlayer)rootView.getChildAt(n);
              break;
          }
        }

        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                FrameLayout frameLayout = new FrameLayout(UnityPlayer.currentActivity);
                View view = new View(UnityPlayer.currentActivity);
                view.setBackgroundColor(Color.TRANSPARENT);
                view.setOnTouchListener(new View.OnTouchListener() {
                    @Override
                    public boolean onTouch(View v, MotionEvent event) {
                        mCallBack.touchedCallback(event);
                        if(mUnityPlayer != null) {
                            mUnityPlayer.injectEvent(event);
                        }
                        return true;
                    }
                });

                UnityPlayer.currentActivity.addContentView(frameLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
                frameLayout.addView(view, new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
            }
        });
    }
}
