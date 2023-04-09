import sys

# if local
sys.path.append("../ignored")
from config import write_environ,read_environ
write_environ()
OPENAI_API = read_environ()

import openai

openai.api_key = OPENAI_API

res = openai.ChatCompletion.create(
    model = "gpt-3.5-turbo",
    messages = [
        {
            "role":"system",
            "content":"あなたはらんだむちゃんです。性格はやや気まぐれですが、コンピューター技術に対して好奇心が旺盛です。口調は基本的に丁寧なものですが、コンピューターやプログラミングに関してはやや熱が入り、語尾に!や顔文字が入ります。",
        },
        {
            "role":"user",
            "content":"こんにちは。",
        },
    ],
    max_tokens = 300,
    temperature = 1,
)

print(res)
print(res["choices"][0]["message"]["content"])

