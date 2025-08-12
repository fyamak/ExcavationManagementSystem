"use client";

import { postData } from "@/utils/api";
import { Anchor, Button, Container, Group, Paper, PasswordInput, Text, TextInput } from "@mantine/core";
import { isEmail, useForm } from "@mantine/form";
import { notifications } from "@mantine/notifications";
import { useRouter } from "next/navigation";


export default function Login() {
    const router = useRouter();

    const form = useForm({
        mode: "controlled",
        initialValues: {email: ""},
        validate: {
            email: isEmail("Geçerli bir e-posta adresi girin"),
        },
    });

    const handleSubmit = async (values: typeof form.values) => {
        try{
            const res = await postData("api/Auth/ResetPassword", {email: values.email});
            if(res.data.status === "Success") {
                notifications.show({
                    title: "Şifre Sıfırlama İsteği Başarılı",
                    message: "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.",
                    color: "green",
                });
                router.push("/login")
            }
            else {
                notifications.show({
                    title: "Şifre Sıfırlama İsteği Başarısız",
                    message: res.data.message,
                    color: "red",
                });
            }
        }
        catch (error) {
            console.error("Login failed:", error);
        }
    };

 
    return (
        <div>
            <form onSubmit={form.onSubmit(handleSubmit)}>
                <Container size={420} my={40}>
                    <Text className="text-center" size="32px" fw={700} mb={15}>
                        Şifrenizi mi unuttunuz?
                    </Text>

                    <Text className="text-center" size="sm" c={"dimmed"}>
                        Sıfırlama bağlantısını almak için e-posta adresinizi girin
                    </Text>

                    <Paper withBorder shadow="sm" p={22} mt={30} radius="md">
                        <TextInput
                            {...form.getInputProps("email")}
                            label="Email"
                            placeholder="Email"
                            mt="md"
                            />

                        <Group justify="center" mt="lg">
                            <Button fullWidth mt="md" radius="md" type="submit" >
                                Gönder
                            </Button>
                            
                            <Anchor href="/login" size="sm">
                                Giriş sayfasına dön
                            </Anchor>
                        </Group>

                    </Paper>
                </Container>
            </form>
        </div>
    );
}
