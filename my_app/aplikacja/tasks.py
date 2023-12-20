from celery import shared_task
from django.core.mail import send_mail, EmailMultiAlternatives
from django.template.loader import render_to_string
from django.utils.html import strip_tags
from .models import User
from django.conf import settings
from django.template import Template, Context
@shared_task
def send_verification_email(user_id):
    try:
        user = User.objects.get(pk=user_id)
        subject = "Zweryfikuj swój adres email"
        verification_link = f'{settings.EMAIL_VERIFICATION_URL}{user_id}/'
        #message = f'Proszę kliknac w poniższy link w celu weryfikacji adresu email: {verification_link}'
        
        #html_message = render_to_string('my_app/templates/email_template.html', {'verification_link': verification_link})
        email_template = """
        <html>
        <head></head>
        <body>
            <p> Dziękujemy za rejestrację! Aby zweryfikować swój adres e-mail, proszę kliknąć w poniższy link: <br>
            <a href='{{ verification_link }}'>Zweryfikuj adres email</a> </p>
        </body>
        </html>
        """
        template = Template(email_template)
        
        context = Context({'verification_link': verification_link})

        html_message = template.render(context)

        plan_message = strip_tags(html_message)
        
        from_email = settings.EMAIL_FROM
        to_email = user.username
        
        email = EmailMultiAlternatives(subject, plan_message, from_email, [to_email])
        email.attach_alternative(html_message,"text/html")
        email.send()
        
        
    except Exception as e:
        print(f"bład {e}")
        raise
    
