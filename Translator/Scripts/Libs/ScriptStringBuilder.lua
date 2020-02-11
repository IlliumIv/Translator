-- https://habr.com/ru/post/99373/

function newBuffer()
	return {n=0}
end

function addString (stack, s)
	tinsert(stack, s)
	for i=stack.n-1, 1, -1 do
		if strlen(stack[i]) > strlen(stack[i+1]) then break end
		stack[i] = stack[i]..tremove(stack)
	end
end

function toString (stack)
	  for i=stack.n-1, 1, -1 do
		    stack[i] = stack[i]..tremove(stack)
	  end
	  return stack[1]
end
