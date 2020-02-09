Global('ChatLog', {})
--------------------------------------------------------------------------------
if not table.normalize then
  function table.normalize(t)
    if t[0] ~= nil then
      table.insert(t, 0, nil)
    end
    return t
  end
end
--------------------------------------------------------------------------------
function ChatLog:CheckContainer()
  if self.wtContainer and self.wtContainer:IsValid() and self.wtContainer:IsVisibleEx() then
    return true
  end
  local wtChatLog = stateMainForm:GetChildUnchecked('ChatLog', false)
  local wtArea = wtChatLog and wtChatLog:GetChildUnchecked('Area', false )
  if wtArea then
    for _, wtPanel in ipairs(table.normalize(wtArea:GetNamedChildren())) do
      self.wtContainer = wtPanel:IsVisibleEx() and wtPanel:GetChildUnchecked('Container', false)
      if self.wtContainer and self.wtContainer:IsVisibleEx() then
        return true
      end
    end
  end
  return false
end

function ChatLog:PushValuedText(valuedText)
  if self:CheckContainer() then
    self.wtContainer:PushFrontValuedText(valuedText)
    for i = 1, (self.wtContainer:GetElementCount() - 100), 1 do
      self.wtContainer:PopBack()
    end
  end
end

function ChatLog:Push(string, classVal, fontSize)
    if fontSize == nil then fontSize = 14 end
    if classVal == nil then classVal = "LogColorYellow" end

    local valuedText = common.CreateValuedText()
    valuedText:SetFormat(userMods.ToWString('<html fontsize="'.. tostring(fontSize) .. '"><rs class="color">[<r name="addonName"/>]: <r name="text"/></rs></html>'))
    valuedText:SetClassVal("color", classVal)
    valuedText:SetVal("text", userMods.ToWString(string))
    valuedText:SetVal("addonName", userMods.ToWString(common.GetAddonName()))

    self:PushValuedText(valuedText)
end

setmetatable(ChatLog, {__call = ChatLog.Push})
--------------------------------------------------------------------------------
