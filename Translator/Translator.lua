function MessagesReceiver(params)
    LogInfo('(' .. userMods.FromWString(params.sender).. '):(' .. userMods.FromWString(params.msg) .. ') TranslatorEndMessage')
end

--------------------------------------------------------------------------------
--- INITIALIZATION
--------------------------------------------------------------------------------
function Init()
    common.RegisterEventHandler(MessagesReceiver, "EVENT_CHAT_MESSAGE")
    common.UnRegisterEventHandler(Init, "EVENT_AVATAR_CREATED")
end
--------------------------------------------------------------------------------

common.RegisterEventHandler(Init, "EVENT_AVATAR_CREATED")
if avatar.IsExist() then
    Init()
end
