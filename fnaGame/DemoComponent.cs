using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace fnaGame
{
    public class DemoComponent : Component
    {
        int _buttonClickCounter;

        public override void OnAddedToEntity()
        {
            // register with the ImGuiMangaer letting it know we want to render some IMGUI
            Core.GetGlobalManager<ImGuiManager>()?.RegisterDrawCommand(ImGuiDraw);
        }

        public override void OnRemovedFromEntity()
        {
            // remove ourselves when we are removed from the Scene
            Core.GetGlobalManager<ImGuiManager>()?.UnregisterDrawCommand(ImGuiDraw);
        }

        void ImGuiDraw()
        {
            // do your actual drawing here
            ImGui.Begin("Your ImGui Window", ImGuiWindowFlags.AlwaysAutoResize);            
            ImGui.Text("This is being drawn in DemoComponent");
            if(ImGui.Button($"Clicked me {_buttonClickCounter} times"))
                _buttonClickCounter++;
            ImGui.End();
        }

    }
}
